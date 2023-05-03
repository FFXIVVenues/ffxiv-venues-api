using FFXIVVenues.Api.Persistence;
using System;

namespace FFXIVVenues.Api.InternalModel
{
    public class Venue : VenueModels.Venue, IEntity
    {

        public string OwningKey { get; set; }
        public string Banner { get; set; }
        public DateTime HiddenUntil { get; set; }

    }
}