using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using FFXIVVenues.Api.Persistence;
using FFXIVVenues.Api.Security;
using FFXIVVenues.Api.Helpers;
using FFXIVVenues.Api.InternalModel;
using FFXIVVenues.Api.Observability;

namespace FFXIVVenues.Api.Controllers
{
    [ApiController]
    public class MediaController : ControllerBase
    {

        private readonly ILogger<MediaController> _logger;
        private readonly IMediaRepository _mediaManager;
        private readonly IObjectRepository _repository;
        private readonly IAuthorizationManager _authorizationManager;
        private readonly IChangeBroker _changeBroker;
        private readonly RollingCache<IEnumerable<VenueModels.Venue>> _cache;

        public MediaController(ILogger<MediaController> logger,
                               IMediaRepository mediaManager,
                               IObjectRepository repository,
                               IAuthorizationManager authorizationManager,
                               IChangeBroker changeBroker,
                               RollingCache<IEnumerable<VenueModels.Venue>> cache)
        {
            this._logger = logger;
            this._mediaManager = mediaManager;
            this._repository = repository;
            this._authorizationManager = authorizationManager;
            this._changeBroker = changeBroker;
            this._cache = cache;
        }

        [HttpGet("/venue/{id}/media")]
        public async Task<ActionResult> GetAsync(string id)
        {
            var venue = _repository.GetById<Venue>(id);
            if (venue == null || _authorizationManager.Check().CanNot(Operation.ReadHidden, venue) && !venue.Approved)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(venue.Banner))
                return new FileStreamResult(System.IO.File.OpenRead("default-banner.jpg"), "image/jpeg");

            var (stream, contentType) = await _mediaManager.Download(venue.Banner, HttpContext.RequestAborted);

            return File(stream, contentType);
        }

        [HttpPut("/venue/{id}/media")]
        public async Task<ActionResult> PutAsync(string id)
        {
            var venue = _repository.GetById<Venue>(id);
            if (venue == null)
                return NotFound();
            if (_authorizationManager.Check().CanNot(Operation.Update, venue))
                return Unauthorized();
            if (Request.ContentLength > 1_048_576)
                return BadRequest();
            if (Request.ContentType?.StartsWith("image/") == false)
                return BadRequest();

            var bannerId = venue.Banner;
            if (string.IsNullOrEmpty(bannerId))
                await _mediaManager.Delete(bannerId);

            venue.Banner = await _mediaManager.Upload(Request.ContentType, Request.Body, HttpContext.RequestAborted);
            this._repository.Upsert(venue);
            this._cache.Clear();
            this._changeBroker.Invoke(ObservableOperation.Update, venue);
            
            return NoContent();
        }

        [HttpDelete("/venue/{id}/media")]
        public ActionResult Delete(string id)
        {
            var venue = _repository.GetById<Venue>(id);
            if (venue == null)
            {
                return NotFound();
            }

            if (_authorizationManager.Check().CanNot(Operation.Delete, venue))
                return Unauthorized();

            if (string.IsNullOrEmpty(venue.Banner))
                return NoContent();

            _mediaManager.Delete(venue.Banner);

            venue.Banner = null;
            _repository.Upsert(venue);
            this._cache.Clear();
            this._changeBroker.Invoke(ObservableOperation.Update, venue);

            return NoContent();
        }

    }
}
