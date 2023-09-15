using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using FFXIVVenues.Api.Security;
using FFXIVVenues.Api.Helpers;
using FFXIVVenues.Api.Observability;
using FFXIVVenues.Api.PersistenceModels.Context;
using FFXIVVenues.Api.PersistenceModels.Media;
using FFXIVVenues.Api.Security.ServiceAuthentication;
using FFXIVVenues.VenueModels.Observability;

namespace FFXIVVenues.Api.Controllers
{
    [ApiController]
    public class MediaController : ControllerBase, IDisposable
    {

        private readonly ILogger<MediaController> _logger;
        private readonly IMediaRepository _mediaManager;
        private readonly IAuthorizationManager _authorizationManager;
        private readonly IChangeBroker _changeBroker;
        private readonly FFXIVVenuesDbContext _db;
        private readonly RollingCache<IEnumerable<VenueModels.Venue>> _cache;

        public MediaController(ILogger<MediaController> logger,
                               IMediaRepository mediaManager,
                               IAuthorizationManager authorizationManager,
                               IChangeBroker changeBroker,
                               IFFXIVVenuesDbContextFactory dbContextFactory,
                               RollingCache<IEnumerable<VenueModels.Venue>> cache)
        {
            this._logger = logger;
            this._mediaManager = mediaManager;
            this._authorizationManager = authorizationManager;
            this._changeBroker = changeBroker;
            this._db = dbContextFactory.Create();
            this._cache = cache;
        }

        [HttpGet("/venue/{id}/media")]
        public async Task<ActionResult> GetAsync(string id)
        {
            var venue = await this._db.Venues.FindAsync(id);
            if (venue == null || venue.Deleted != null || _authorizationManager.Check().CanNot(Operation.Read, venue))
                return NotFound();

            if (string.IsNullOrEmpty(venue.Banner))
                return new FileStreamResult(System.IO.File.OpenRead("default-banner.jpg"), "image/jpeg");

            var (stream, contentType) = await _mediaManager.Download(venue.Banner, HttpContext.RequestAborted);

            return File(stream, contentType);
        }

        [HttpPut("/venue/{id}/media")]
        public async Task<ActionResult> PutAsync(string id)
        {
            var venue = await this._db.Venues.FindAsync(id);

            if (venue == null)
                return NotFound();
            if (_authorizationManager.Check().CanNot(Operation.Update, venue))
                return Unauthorized();
            if (venue.Deleted != null)
                return Unauthorized("Cannot PUT to a deleted venue.");
            if (Request.ContentLength > 10_048_576)
                return BadRequest();
            if (Request.ContentType?.StartsWith("image/") == false)
                return BadRequest();

            if (!string.IsNullOrEmpty(venue.Banner))
                await _mediaManager.Delete(venue.Banner);

            venue.Banner = await _mediaManager.Upload(Request.ContentType, Request.Body, HttpContext.RequestAborted);
            venue.LastModified = DateTimeOffset.UtcNow;
            this._db.Venues.Update(venue);
            await this._db.SaveChangesAsync();
            
            this._cache.Clear();
            this._changeBroker.Queue(ObservableOperation.Update, venue);
            
            return NoContent();
        }

        [HttpDelete("/venue/{id}/media")]
        public async Task<ActionResult> Delete(string id)
        {
            var venue = await this._db.Venues.FindAsync(id);
            if (venue == null || venue.Deleted != null)
                return NotFound();

            if (_authorizationManager.Check().CanNot(Operation.Delete, venue))
                return Unauthorized();

            if (string.IsNullOrEmpty(venue.Banner))
                return NoContent();

            await _mediaManager.Delete(venue.Banner);

            venue.Banner = null;
            this._db.Venues.Update(venue);
            await this._db.SaveChangesAsync();
            
            this._cache.Clear();
            this._changeBroker.Queue(ObservableOperation.Update, venue);

            return NoContent();
        }

        public void Dispose()
        {
            _db?.Dispose();
        }
    }
}
