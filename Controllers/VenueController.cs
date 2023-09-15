using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Text;
using System.Threading;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Reflection;
using AutoMapper;
using FFXIVVenues.Api.Controllers.ArgModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FFXIVVenues.Api.Security;
using FFXIVVenues.Api.Observability;
using FFXIVVenues.Api.Helpers;
using FFXIVVenues.Api.PersistenceModels.Context;
using FFXIVVenues.Api.PersistenceModels.Entities;
using FFXIVVenues.Api.PersistenceModels.Mapping;
using FFXIVVenues.Api.PersistenceModels.Media;
using FFXIVVenues.Api.Security.ServiceAuthentication;
using FFXIVVenues.VenueModels.Observability;
using Microsoft.AspNetCore.Http;

using VenueObservation = FFXIVVenues.Api.Observability.VenueObservation;
using Dto = FFXIVVenues.VenueModels;

namespace FFXIVVenues.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VenueController : ControllerBase, IDisposable
    {

        private readonly IMediaRepository _mediaManager;
        private readonly IMapper _modelMapper;
        private readonly IMapper _modelProjector;
        private readonly IAuthorizationManager _authorizationManager;
        private readonly IChangeBroker _changeBroker;
        private readonly FFXIVVenuesDbContext _db;
        private readonly RollingCache<IEnumerable<Dto.Venue>> _cache;

        public VenueController(IMediaRepository mediaManager,
                               IAuthorizationManager authorizationManager,
                               IMapFactory mapFactory,
                               IChangeBroker changeBroker, 
                               IFFXIVVenuesDbContextFactory dbContextFactory,
                               RollingCache<IEnumerable<Dto.Venue>> cache)
        {
            this._mediaManager = mediaManager;
            this._modelMapper = mapFactory.GetModelMapper();
            this._modelProjector = mapFactory.GetModelProjector();
            this._authorizationManager = authorizationManager;
            this._changeBroker = changeBroker;
            this._db = dbContextFactory.Create();
            this._cache = cache;
        }

        [HttpGet]
        public IEnumerable<VenueModels.Venue> Get([FromQuery] VenueQueryArgs queryArgs)
        {
            var query = this._db.Venues.AsQueryable();
            query = queryArgs.ApplyDomainQueryArgs(query);
            query = query.Where(v => v.Deleted == null);
            query = this._authorizationManager.Check().Can(Operation.Read, query);
            var dtos = this._modelProjector.ProjectTo<Dto.Venue>(query);
            return queryArgs.ApplyDtoQueryArgs(dtos);
        }

        [HttpGet("{id}")]
        public ActionResult<VenueModels.Venue> GetById(string id, bool? recordView = true)
        {
            var venue = this._db.Venues.Find(id);
            if (venue == null || venue.Deleted != null || _authorizationManager.Check().CanNot(Operation.Read, venue))
                return NotFound();

            if (recordView == null || recordView == true)
            {
                this._db.VenueViews.Add(new VenueView(venue));
                this._db.SaveChanges();
            }
            
            return this._modelMapper.Map<VenueModels.Venue>(venue);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(string id, [FromBody] VenueModels.Venue venue)
        {
            if (venue.Id != id)
                return BadRequest("Venue ID does not match.");

            var existingVenue = this._db.Venues.Include(v => v.Openings).FirstOrDefault(v => v.Id == id);
            
            if (existingVenue == null)
            {
                if (_authorizationManager.Check().CanNot(Operation.Create))
                    return Unauthorized();

                var owningKey = _authorizationManager.GetKeyString();
                var newInternalVenue = this._modelMapper.Map<PersistenceModels.Entities.Venues.Venue>(venue);
                newInternalVenue.ScopeKey = owningKey;
                this._db.Venues.Add(newInternalVenue);
                await this._db.SaveChangesAsync();
                
                this._changeBroker.Queue(ObservableOperation.Create, newInternalVenue);
                this._cache.Clear();
                
                return Ok(this._modelMapper.Map<VenueModels.Venue>(newInternalVenue));
            }

            if (_authorizationManager.Check().CanNot(Operation.Update, existingVenue))
                return Unauthorized();

            if (existingVenue.Deleted != null)
                return Unauthorized("Cannot PUT to a deleted venue.");

            this._modelMapper.Map(venue, existingVenue);
            existingVenue.LastModified = DateTimeOffset.UtcNow;
            this._db.Venues.Update(existingVenue);
            await this._db.SaveChangesAsync();
            
            this._changeBroker.Queue(ObservableOperation.Update, existingVenue);
            this._cache.Clear();
            
            return Ok(this._modelMapper.Map<Dto.Venue>(existingVenue));
        }

        [HttpDelete("{id}")]
        public ActionResult<VenueModels.Venue> Delete(string id)
        {
            var venue = this._db.Venues.Find(id);
            if (venue == null || venue.Deleted != null)
                return NotFound();
            if (_authorizationManager.Check().CanNot(Operation.Delete, venue))
                return Unauthorized();
            if (venue.Banner != null)
            {
                _mediaManager.Delete(venue.Banner);
                venue.Banner = null;
            }
            venue.Deleted = DateTimeOffset.UtcNow;
            this._db.Venues.Update(venue);
            this._db.SaveChanges();
            
            this._changeBroker.Queue(ObservableOperation.Delete, venue);
            this._cache.Clear();
            
            return Ok(this._modelMapper.Map<Dto.Venue>(venue));
        }
        
        [HttpGet("{id}/approved")]
        public ActionResult Approved(string id)
        {
            var venue = this._db.Venues.Find(id);
            if (venue == null || venue.Deleted != null)
                return NotFound();

            if (_authorizationManager.Check().CanNot(Operation.Approve, venue))
                return Unauthorized();

            this._cache.Clear();
            return Ok(venue.Approved);
        }

        [HttpPut("{id}/approved")]
        public async Task<ActionResult> Approved(string id, [FromBody] bool approved)
        {
            var venue = await this._db.Venues.FindAsync(id);
            if (venue == null || venue.Deleted != null)
                return NotFound();

            if (_authorizationManager.Check().CanNot(Operation.Approve, venue))
                return Unauthorized();

            if (venue.Approved != approved)
            {
                venue.Approved = approved;
                this._db.Venues.Update(venue);
                await this._db.SaveChangesAsync();
                
                this._cache.Clear();
                this._changeBroker.Queue(ObservableOperation.Update, venue);
            }
            
            return Ok(venue.Approved);
        }

        private static PropertyInfo _addedField = typeof(PersistenceModels.Entities.Venues.Venue).GetProperty("Added");

        [HttpPut("{id}/added")]
        public ActionResult Added(string id, [FromBody] DateTime added)
        {
            
            
            var venue = this._db.Venues.Find(id);
            if (venue == null || venue.Deleted != null)
                return NotFound();

            if (_authorizationManager.Check().CanNot(Operation.Approve, venue))
                return Unauthorized();

            venue.Added = new DateTimeOffset(added.ToUniversalTime());
            this._db.Venues.Update(venue);
            this._db.SaveChanges();

            this._changeBroker.Queue(ObservableOperation.Update, venue);
            this._cache.Clear();
            return Ok(venue.Added);
        }

        [HttpPost("{id}/open")]
        public ActionResult Open(string id, [FromBody] DateTime until)
        {
            var venue = this._db.Venues.Find(id);
            if (venue == null || venue.Deleted != null)
                return NotFound();

            if (_authorizationManager.Check().CanNot(Operation.Update, venue))
                return Unauthorized();

            if (until > DateTime.UtcNow.AddHours(6))
                return BadRequest("Cannot open for more than 6 hours ahead.");
            
            var newOverrides = venue.OpenOverrides.Where(o => o.Start > until).ToList();
            newOverrides.Add(new()
            {
                Open = true,
                Start = DateTime.UtcNow,
                End = until
            });
            venue.OpenOverrides = newOverrides;

            this._db.Venues.Update(venue);
            this._db.SaveChanges();
                
            this._changeBroker.Queue(ObservableOperation.Update, venue);
            this._cache.Clear();
            
            return Ok(this._modelMapper.Map<Dto.Venue>(venue));
        }

        [HttpPost("{id}/close")]
        public ActionResult Close(string id, [FromBody] DateTime until)
        {
            var venue = this._db.Venues.Find(id);
            if (venue == null || venue.Deleted != null)
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

            this._db.Venues.Update(venue);
            this._db.SaveChanges();
            
            this._changeBroker.Queue(ObservableOperation.Update, venue);
            this._cache.Clear();
            return Ok(this._modelMapper.Map<Dto.Venue>(venue));
        }

        [HttpGet("observe")]
        public async Task Observe()
        {
            if (!this.HttpContext.WebSockets.IsWebSocketRequest)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            var webSocket = await this.ControllerContext.HttpContext.WebSockets.AcceptWebSocketAsync();

            Action removeExistingObserver = null;
            
            var buffer = new byte[1024 * 4];
            while (true)
            {
                WebSocketReceiveResult result = null;
                try
                {
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }
                catch (WebSocketException)
                {
                    removeExistingObserver?.Invoke();
                    return;
                }
                if (result.CloseStatus.HasValue)
                {
                    removeExistingObserver?.Invoke();
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                    break;
                }

                var message = Encoding.UTF8.GetString(new ReadOnlySpan<byte>(buffer, 0, result.Count));
                var observer = JsonSerializer.Deserialize<Observer>(message);
                if (observer == null)
                    continue;

                observer.ObserverAction = (op, venue) =>
                {
                    var change = VenueObservation.FromVenue(op, venue);
                    var payload = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(change));
                    return webSocket.SendAsync(payload, WebSocketMessageType.Text, true, CancellationToken.None);
                };
                removeExistingObserver?.Invoke();
                removeExistingObserver = this._changeBroker.Observe(observer, InvocationKind.Delayed);
            }
        }

        public void Dispose()
        {
            _db?.Dispose();
        }
    }
}
