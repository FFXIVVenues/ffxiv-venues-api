using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using FFXIVVenues.Api.Persistence;
using FFXIVVenues.Api.Security;
using FFXIVVenues.Api.Helpers;
using FFXIVVenues.Api.InternalModel;

namespace FFXIVVenues.Api.Controllers
{
    [ApiController]
    public class MediaController : ControllerBase
    {

        private readonly ILogger<MediaController> _logger;
        private readonly IMediaRepository _mediaManager;
        private readonly IObjectRepository _repository;
        private readonly IAuthorizationManager _authorizationManager;

        public MediaController(ILogger<MediaController> logger,
                               IMediaRepository mediaManager,
                               IObjectRepository repository,
                               IAuthorizationManager authorizationManager)
        {
            _logger = logger;
            _mediaManager = mediaManager;
            _repository = repository;
            _authorizationManager = authorizationManager;
        }

        [HttpGet("/venue/{id}/media")]
        public async Task<ActionResult> GetAsync(string id)
        {
            var venue = _repository.GetById<Venue>(id);
            if (venue == null || _authorizationManager.Check().CanNot(Operation.ReadInternal, venue) && !venue.Approved)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(venue.Banner))
                return NotFound();

            var blob = await _mediaManager.Download(venue.Banner, HttpContext.RequestAborted);

            return File(blob.Content, blob.Details.ContentType);
        }

        [HttpPut("/venue/{id}/media")]
        public async Task<ActionResult> PutAsync(string id)
        {
            var venue = _repository.GetById<Venue>(id);
            if (venue == null)
            {
                return NotFound();
            }

            if (_authorizationManager.Check().CanNot(Operation.Update, venue))
                return Unauthorized();

            if (Request.ContentLength > 1_048_576)
                return BadRequest();

            if (!Request.ContentType.StartsWith("image/"))
                return BadRequest();


            var bannerId = venue.Banner;
            if (string.IsNullOrEmpty(bannerId))
                bannerId = IdHelper.GenerateId();

            await _mediaManager.Upload(bannerId, Request.ContentType, Request.Body, HttpContext.RequestAborted);

            if (venue.Banner != bannerId)
            {
                venue.Banner = bannerId;
                _repository.Upsert(venue);
            }

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

            return NoContent();
        }

    }
}
