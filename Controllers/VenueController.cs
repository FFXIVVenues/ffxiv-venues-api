using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using FFXIVVenues.Api.Persistence;
using FFXIVVenues.Api.Security;
using FFXIVVenues.Api.Observability;
using System.Text.Json;
using System.Text;
using System.Threading;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Reflection;

namespace FFXIVVenues.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VenueController : ControllerBase
    {

        private readonly IObjectRepository _repository;
        private readonly IMediaRepository _mediaManager;
        private readonly IAuthorizationManager _authorizationManager;
        private readonly IChangeBroker _changeBroker;

        public VenueController(IObjectRepository repository,
                               IMediaRepository mediaManager,
                               IAuthorizationManager authorizationManager,
                               IChangeBroker changeBroker)
        {
            this._repository = repository;
            this._mediaManager = mediaManager;
            this._authorizationManager = authorizationManager;
            this._changeBroker = changeBroker;
        }

        [HttpGet]
        public IEnumerable<VenueModels.V2022.Venue> Get(string search = null,
                                                        string manager = null,
                                                        string dataCenter = null,
                                                        string world = null,
                                                        string tags = null,
                                                        bool? approved = null,
                                                        bool? open = null)
        {
            var query = _repository.GetAll<InternalModel.Venue>();
            if (search != null)
                query = query.Where(v => v.Name.ToLower().Contains(search.ToLower()));
            if (manager != null)
                query = query.Where(v => v.Managers.Contains(manager));
            if (dataCenter != null)
                query = query.Where(v => string.Equals(v.Location.DataCenter, dataCenter, StringComparison.InvariantCultureIgnoreCase));
            if (world != null)
                query = query.Where(v => string.Equals(v.Location.World, world, StringComparison.InvariantCultureIgnoreCase));
            if (tags != null)
            {
                var splitTags = tags.Split(',');
                query = query.Where(v => splitTags.All(t => v.Tags.Contains(t, StringComparer.OrdinalIgnoreCase)));
            }
            if (approved != null)
                query = query.Where(v => v.Approved == approved);
            if (open != null)
                query = query.Where(v => v.IsOpen() == open);

            query = query.ToList().Where(v => v.Approved && v.HiddenUntil < DateTime.UtcNow || this._authorizationManager.Check().Can(Operation.ReadHidden, v)).AsQueryable();

            return query.Select(v => v.ToPublicModel());
        }

        [HttpGet("{id}")]
        public ActionResult<VenueModels.V2022.Venue> GetById(string id, bool? recordView = true)
        {
            var venue = _repository.GetById<InternalModel.Venue>(id);
            if (venue == null || _authorizationManager.Check().CanNot(Operation.ReadHidden, venue) && !venue.Approved)
            {
                return NotFound();
            }
            if (recordView == null || recordView == true)
                _repository.Upsert(new InternalModel.ViewRecord(id));
            return venue.ToPublicModel();
        }

        [HttpPut("{id}")]
        public ActionResult Put(string id, [FromBody] VenueModels.V2022.Venue venue)
        {
            if (venue.Id != id)
                return BadRequest("Venue ID does not match.");

            var existingVenue = _repository.GetById<InternalModel.Venue>(id);
            if (existingVenue == null)
            {
                if (_authorizationManager.Check().CanNot(Operation.Create, existingVenue))
                    return Unauthorized();

                var owningKey = _authorizationManager.GetKey();
                _repository.Upsert(InternalModel.Venue.CreateFromPublicModel(venue, owningKey));
                return Ok(venue);
            }

            if (_authorizationManager.Check().CanNot(Operation.Update, existingVenue))
                return Unauthorized();

            _repository.Upsert(existingVenue.UpdateFromPublicModel(venue));
            this._changeBroker.Invoke(ObservableOperation.Update, venue);

            return Ok(venue);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            var venue = _repository.GetById<InternalModel.Venue>(id);
            if (venue == null)
                return NotFound();
            if (_authorizationManager.Check().CanNot(Operation.Delete, venue))
                return Unauthorized();
            if (venue.Banner != null)
                _mediaManager.Delete(venue.Banner);
            _repository.Delete<InternalModel.Venue>(id);
            this._changeBroker.Invoke(ObservableOperation.Delete, venue);
            return Ok(venue);
        }


        [HttpGet("{id}/approved")]
        public ActionResult Approved(string id)
        {
            var venue = _repository.GetById<InternalModel.Venue>(id);
            if (venue == null)
                return NotFound();

            if (_authorizationManager.Check().CanNot(Operation.Approve, venue))
                return Unauthorized();

            return Ok(venue.Approved);
        }

        [HttpPut("{id}/approved")]
        public ActionResult Approved(string id, [FromBody] bool approved)
        {
            var venue = _repository.GetById<InternalModel.Venue>(id);
            if (venue == null)
                return NotFound();

            if (_authorizationManager.Check().CanNot(Operation.Approve, venue))
                return Unauthorized();

            if (venue.Approved != approved)
            {
                venue.Approved = approved;
                this._repository.Upsert(venue);
                this._changeBroker.Invoke(approved ? ObservableOperation.Create : ObservableOperation.Delete, venue);
            }
            return Ok(venue);
        }

        private static PropertyInfo _addedField = typeof(InternalModel.Venue).GetProperty("Added");
        [HttpPut("{id}/added")]
        public ActionResult Added(string id, [FromBody] DateTime added)
        {
            var venue = _repository.GetById<InternalModel.Venue>(id);
            if (venue == null)
                return NotFound();

            if (_authorizationManager.Check().CanNot(Operation.Approve, venue))
                return Unauthorized();

            _addedField.SetValue(venue, added);

            this._changeBroker.Invoke(ObservableOperation.Update, venue);
            _repository.Upsert(venue);
            return Ok(venue);
        }

        [HttpPut("{id}/hiddenUntil")]
        public ActionResult HiddenUntil(string id, [FromBody] DateTime until)
        {
            var venue = _repository.GetById<InternalModel.Venue>(id);
            if (venue == null)
                return NotFound();

            if (_authorizationManager.Check().CanNot(Operation.Update, venue))
                return Unauthorized();

            venue.HiddenUntil = until;

            this._changeBroker.Invoke(ObservableOperation.Update, venue);
            _repository.Upsert(venue);
            return Ok(venue);
        }

        [HttpPost("{id}/open")]
        public ActionResult Open(string id)
        {
            var venue = _repository.GetById<InternalModel.Venue>(id);
            if (venue == null)
                return NotFound();

            if (_authorizationManager.Check().CanNot(Operation.Update, venue))
                return Unauthorized();


            var newOverrides = venue.OpenOverrides.Where(o => o.Start > DateTime.UtcNow.AddHours(2.5)).ToList();
            newOverrides.Add(new()
            {
                Open = true,
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddHours(2.5)
            });
            venue.OpenOverrides = newOverrides;

            this._changeBroker.Invoke(ObservableOperation.Update, venue);
            _repository.Upsert(venue);
            return Ok(venue);
        }

        [HttpPost("{id}/close")]
        public ActionResult Close(string id, [FromBody]DateTime until)
        {
            var venue = _repository.GetById<InternalModel.Venue>(id);
            if (venue == null)
                return NotFound();

            if (_authorizationManager.Check().CanNot(Operation.Update, venue)) 
                return Unauthorized();


            var newOverrides = venue.OpenOverrides.Where(o => o.Start > until).ToList();
            newOverrides.Add(new()
            {
                Open = false,
                Start = DateTime.UtcNow,
                End = until
            });
            venue.OpenOverrides = newOverrides;

            this._changeBroker.Invoke(ObservableOperation.Update, venue);
            _repository.Upsert(venue);
            return Ok(venue);
        }

        [HttpPut("observe")]
        public async Task<ActionResult> Observe([FromBody] Observer observer)
        {
            if (this.ControllerContext.HttpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await this.ControllerContext.HttpContext.WebSockets.AcceptWebSocketAsync();

                Action removeObserver = null;
                observer.ObserverAction = async (op, venue) =>
                {
                    if (webSocket.State == WebSocketState.Closed ||
                        webSocket.State == WebSocketState.Aborted) 
                    {
                        removeObserver();
                        return;
                    }
                    
                    var change = new
                    {
                        operation = op.ToString(),
                        subject = venue.Id
                    };
                    var payload = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(change));
                    await webSocket.SendAsync(payload, WebSocketMessageType.Text, true, CancellationToken.None);
                };
                removeObserver = this._changeBroker.Observe(observer);

                var buffer = new byte[1024 * 4];
                while (true)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        removeObserver();
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                        break;
                    }
                }

                return this.NoContent();
            }
            else
                return this.BadRequest();
        }

    }
}
