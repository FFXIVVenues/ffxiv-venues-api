using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
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
using AutoMapper;
using FFXIVVenues.Api.Helpers;
using FFXIVVenues.Api.PersistenceModels;
using FFXIVVenues.Api.PersistenceModels.Context;
using FFXIVVenues.Api.PersistenceModels.Entities;
using FFXIVVenues.VenueModels;
using FFXIVVenues.VenueModels.Observability;
using Microsoft.AspNetCore.Http;
using VenueObservation = FFXIVVenues.Api.Observability.VenueObservation;

namespace FFXIVVenues.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VenueController : ControllerBase
    {

        private readonly IMediaRepository _mediaManager;
        private readonly IMapper _modelMapper;
        private readonly IAuthorizationManager _authorizationManager;
        private readonly IChangeBroker _changeBroker;
        private readonly RollingCache<IEnumerable<Venue>> _cache;

        public VenueController(IMediaRepository mediaManager,
                               IAuthorizationManager authorizationManager,
                               IMapper modelMapper,
                               IChangeBroker changeBroker, 
                               RollingCache<IEnumerable<Venue>> cache)
        {
            this._mediaManager = mediaManager;
            this._modelMapper = modelMapper;
            this._authorizationManager = authorizationManager;
            this._changeBroker = changeBroker;
            this._cache = cache;
        }

        [HttpGet]
        // todo: Fix this insanity
        public IEnumerable<VenueModels.Venue> Get(string search = null,
                                                    string manager = null,
                                                    string dataCenter = null,
                                                    string world = null,
                                                    string tags = null,
                                                    bool? hasBanner = null,
                                                    bool? approved = null,
                                                    bool? open = null)
        {
            var cacheKey = $"*|{search}|{manager}|{world}|{tags}|{hasBanner}|{approved}|{open}";
            var cache = this._cache.Get(cacheKey);
            if (cache != null)
                return cache;

            using var db = new VenuesContext();
            
            var query = db.Venues.AsQueryable();
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
            if (hasBanner != null)
                query = query.Where(v => hasBanner.Value == (v.Banner != null));
            
            query = query.ToList()
                .Where(v => v.Approved && v.HiddenUntil < DateTime.UtcNow || this._authorizationManager.Check().Can(Operation.ReadHidden, v))
                .AsQueryable();

            var projectedQuery = this._modelMapper.ProjectTo<Venue>(query);
            if (open != null)
                projectedQuery = projectedQuery.Where(v => v.IsOpen() == open);
            var results = projectedQuery.ToList();
            
            this._cache.Set(cacheKey, results);
            return results;
        }

        [HttpGet("{id}")]
        public ActionResult<VenueModels.Venue> GetById(string id, bool? recordView = true)
        {
            using var db = new VenuesContext();
            var venue = db.Venues.Find(id);
            
            if (venue == null || _authorizationManager.Check().CanNot(Operation.ReadHidden, venue) && !venue.Approved)
                return NotFound();

            if (recordView == null || recordView == true)
            {
                db.VenueViews.Add(new VenueView(venue));
                db.SaveChanges();
            }
            
            return this._modelMapper.Map<VenueModels.Venue>(venue);
        }

        [HttpPut("{id}")]
        public ActionResult Put(string id, [FromBody] VenueModels.Venue venue)
        {
            using var db = new VenuesContext();
            
            if (venue.Id != id)
                return BadRequest("Venue ID does not match.");

            var existingVenue = db.Venues.Find(id);
            if (existingVenue == null)
            {
                if (_authorizationManager.Check().CanNot(Operation.Create, existingVenue))
                    return Unauthorized();

                var owningKey = _authorizationManager.GetKey();
                var newInternalVenue = this._modelMapper.Map<PersistenceModels.Entities.Venues.Venue>(venue);
                newInternalVenue.ScopeKey = owningKey;
                db.Venues.Add(newInternalVenue);
                db.SaveChanges();
                
                this._changeBroker.Queue(ObservableOperation.Create, newInternalVenue);
                this._cache.Clear();
                
                return Ok(this._modelMapper.Map<VenueModels.Venue>(newInternalVenue));
            }

            if (_authorizationManager.Check().CanNot(Operation.Update, existingVenue))
                return Unauthorized();

            this._modelMapper.Map(venue, existingVenue);
            existingVenue.LastModified = DateTime.UtcNow;
            db.Venues.Update(existingVenue);
            db.SaveChanges();
            
            this._changeBroker.Queue(ObservableOperation.Update, existingVenue);
            this._cache.Clear();
            
            return Ok(this._modelMapper.Map<Venue>(existingVenue));
        }

        [HttpDelete("{id}")]
        public ActionResult<VenueModels.Venue> Delete(string id)
        {
            using var db = new VenuesContext();
            
            var venue = db.Venues.Find(id);
            if (venue == null)
                return NotFound();
            if (_authorizationManager.Check().CanNot(Operation.Delete, venue))
                return Unauthorized();
            if (venue.Banner != null)
                _mediaManager.Delete(venue.Banner);
            db.Venues.Remove(venue);
            db.SaveChanges();
            
            this._changeBroker.Queue(ObservableOperation.Delete, venue);
            this._cache.Clear();
            
            return Ok(this._modelMapper.Map<Venue>(venue));
        }
        
        [HttpGet("{id}/approved")]
        public ActionResult Approved(string id)
        {
            using var db = new VenuesContext();

            var venue = db.Venues.Find(id);
            if (venue == null)
                return NotFound();

            if (_authorizationManager.Check().CanNot(Operation.Approve, venue))
                return Unauthorized();

            this._cache.Clear();
            return Ok(venue.Approved);
        }

        [HttpPut("{id}/approved")]
        public ActionResult Approved(string id, [FromBody] bool approved)
        {
            using var db = new VenuesContext();
            
            var venue = db.Venues.Find(id);
            if (venue == null)
                return NotFound();

            if (_authorizationManager.Check().CanNot(Operation.Approve, venue))
                return Unauthorized();

            if (venue.Approved != approved)
            {
                venue.Approved = approved;
                db.Venues.Update(venue);
                
                this._cache.Clear();
                this._changeBroker.Queue(ObservableOperation.Update, venue);
            }
            
            return Ok(venue.Approved);
        }

        private static PropertyInfo _addedField = typeof(PersistenceModels.Entities.Venues.Venue).GetProperty("Added");

        [HttpPut("{id}/added")]
        public ActionResult Added(string id, [FromBody] DateTime added)
        {
            using var db = new VenuesContext();
            
            var venue = db.Venues.Find(id);
            if (venue == null)
                return NotFound();

            if (_authorizationManager.Check().CanNot(Operation.Approve, venue))
                return Unauthorized();

            venue.Added = added;
            db.Venues.Update(venue);
            db.SaveChanges();

            this._changeBroker.Queue(ObservableOperation.Update, venue);
            this._cache.Clear();
            return Ok(venue.Added);
        }

        [HttpPut("{id}/hiddenUntil")]
        public ActionResult HiddenUntil(string id, [FromBody] DateTime until)
        {
            using var db = new VenuesContext();

            var venue = db.Venues.Find(id);
            if (venue == null)
                return NotFound();

            if (_authorizationManager.Check().CanNot(Operation.Update, venue))
                return Unauthorized();

            venue.HiddenUntil = until;
            db.Venues.Update(venue);
            db.SaveChanges();
            
            this._changeBroker.Queue(ObservableOperation.Update, venue);
            this._cache.Clear();
            return Ok(venue.HiddenUntil);
        }

        [HttpPost("{id}/open")]
        public ActionResult Open(string id, [FromBody] DateTime until)
        {
            using var db = new VenuesContext();

            var venue = db.Venues.Find(id);
            if (venue == null)
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

            db.Venues.Update(venue);
            db.SaveChanges();
                
            this._changeBroker.Queue(ObservableOperation.Update, venue);
            this._cache.Clear();
            
            return Ok(this._modelMapper.Map<Venue>(venue));
        }

        [HttpPost("{id}/close")]
        public ActionResult Close(string id, [FromBody] DateTime until)
        {
            using var db = new VenuesContext();
            
            var venue = db.Venues.Find(id);
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

            db.Venues.Update(venue);
            db.SaveChanges();
            
            this._changeBroker.Queue(ObservableOperation.Update, venue);
            this._cache.Clear();
            return Ok(this._modelMapper.Map<Venue>(venue));
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

    }
}
