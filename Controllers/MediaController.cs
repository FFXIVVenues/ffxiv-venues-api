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
using FFXIVVenues.VenueModels.Observability;

namespace FFXIVVenues.Api.Controllers
{
    [ApiController]
    public class MediaController(
        IMediaRepository mediaManager,
        IAuthorizationManager authorizationManager,
        IChangeBroker changeBroker,
        IFFXIVVenuesDbContextFactory dbContextFactory,
        RollingCache<IEnumerable<VenueModels.Venue>> cache)
        : ControllerBase, IDisposable
    {

        private readonly FFXIVVenuesDbContext _db = dbContextFactory.Create();

        [HttpGet("/venue/{id}/media")]
        public async Task<ActionResult> GetAsync(string id)
        {
            var venue = await this._db.Venues.FindAsync(id);
            if (venue is null || venue.Deleted is not null || authorizationManager.Check().CanNot(Operation.Read, venue))
                return NotFound();

            if (string.IsNullOrEmpty(venue.Banner))
                return new FileStreamResult(System.IO.File.OpenRead("default-banner.jpg"), "image/jpeg");

            var (stream, contentType) = await mediaManager.Download(venue.Banner, HttpContext.RequestAborted);

            return File(stream, contentType);
        }

        [HttpPut("/venue/{id}/media")]
        public async Task<ActionResult> PutAsync(string id)
        {
            var venue = await this._db.Venues.FindAsync(id);

            if (venue is null)
                return NotFound();
            if (authorizationManager.Check().CanNot(Operation.Update, venue))
                return Unauthorized();
            if (venue.Deleted is not null)
                return Unauthorized("Cannot PUT to a deleted venue.");
            if (Request.ContentLength > 10_048_576)
                return BadRequest();
            if (Request.ContentType?.StartsWith("image/") == false)
                return BadRequest();

            if (!string.IsNullOrEmpty(venue.Banner))
                await mediaManager.Delete(venue.Banner);

            venue.Banner = await mediaManager.Upload(Request.ContentType, Request.Body, HttpContext.RequestAborted);
            venue.LastModified = DateTimeOffset.UtcNow;
            this._db.Venues.Update(venue);
            await this._db.SaveChangesAsync();
            
            cache.Clear();
            changeBroker.Queue(ObservableOperation.Update, venue);
            
            return NoContent();
        }

        [HttpDelete("/venue/{id}/media")]
        public async Task<ActionResult> Delete(string id)
        {
            var venue = await this._db.Venues.FindAsync(id);
            if (venue is null || venue.Deleted is not null)
                return NotFound();

            if (authorizationManager.Check().CanNot(Operation.Delete, venue))
                return Unauthorized();

            if (string.IsNullOrEmpty(venue.Banner))
                return NoContent();

            await mediaManager.Delete(venue.Banner);

            venue.Banner = null;
            this._db.Venues.Update(venue);
            await this._db.SaveChangesAsync();
            
            cache.Clear();
            changeBroker.Queue(ObservableOperation.Update, venue);

            return NoContent();
        }

        public void Dispose()
        {
            _db?.Dispose();
        }
    }
}
