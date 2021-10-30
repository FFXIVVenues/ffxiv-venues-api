using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using FFXIVVenues.Api.Persistence;
using FFXIVVenues.Api.Security;
using FFXIVVenues.VenueModels.V2022;

namespace FFXIVVenues.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VenueController : ControllerBase
    {

        private readonly IObjectRepository _repository;
        private readonly IMediaRepository _mediaManager;
        private readonly IAuthorizationManager _authorizationManager;

        public VenueController(IObjectRepository repository,
                               IMediaRepository mediaManager,
                               IAuthorizationManager authorizationManager)
        {
            _repository = repository;
            _mediaManager = mediaManager;
            _authorizationManager = authorizationManager;
        }

        [HttpGet]
        public IEnumerable<VenueModels.V2022.Venue> Get(string manager = null)
        {
            var query = _repository.GetAll<InternalModel.Venue>();
            if (manager != null)
                query = query.Where(v => v.Managers.Contains(manager));
            if (_authorizationManager.Check().CanNot(Operation.Approve))
                query = query.Where(v => v.Approved);

            return query.Select(v => v.ToPublicModel());
        }

        [HttpGet("{id}")]
        public ActionResult<VenueModels.V2022.Venue> GetById(string id)
        {
            var venue = _repository.GetById<InternalModel.Venue>(id);
            if (venue == null || _authorizationManager.Check().CanNot(Operation.Read, venue) && !venue.Approved)
            {
                return NotFound();
            }
            _repository.Upsert(new InternalModel.ViewRecord(id));
            return venue.ToPublicModel();
        }

        [HttpPut("{id}")]
        public ActionResult Put(string id, [FromBody] VenueModels.V2022.Venue venue)
        {
            venue.Id = id;

            var existingVenue = _repository.GetById<InternalModel.Venue>(id);
            if (existingVenue == null)
            {
                if (_authorizationManager.Check().CanNot(Operation.Create, existingVenue))
                    return Unauthorized();

                var owningKey = _authorizationManager.GetKey();
                _repository.Upsert(InternalModel.Venue.CreateFromPublicModel(venue, owningKey));
                return NoContent();
            }

            if (_authorizationManager.Check().CanNot(Operation.Update, existingVenue))
                return Unauthorized();

            _repository.Upsert(existingVenue.UpdateFromPublicModel(venue));
            return NoContent();
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
            return NoContent();
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
                _repository.Upsert(venue);
            }
            return NoContent();
        }

        [HttpPut("{id}/open")]
        public ActionResult Open(string id, [FromBody] bool open)
        {
            var venue = _repository.GetById<InternalModel.Venue>(id);
            if (venue == null)
                return NotFound();

            if (_authorizationManager.Check().CanNot(Operation.Update, venue))
                return Unauthorized();


            var newOverrides = venue.OpenOverrides.Where(o => o.Start > DateTime.UtcNow.AddHours(open ? 2.5 : 18)).ToList();
            newOverrides.Add(new()
            {
                Open = open,
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddHours(open ? 2.5 : 18)
            });
            venue.OpenOverrides = newOverrides;

            _repository.Upsert(venue);
            return NoContent();
        }

    }
}
