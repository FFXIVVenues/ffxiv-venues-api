using FFXIVVenues.Api.Persistence;
using System;

namespace FFXIVVenues.Api.InternalModel
{
    public class Venue : VenueModels.V2022.Venue, IEntity
    {

        public string OwningKey { get; set; }
        public string Banner { get; set; }
        public DateTime HiddenUntil { get; set; }
        public bool Approved { get; set; }

        public Venue UpdateFromPublicModel(VenueModels.V2022.Venue venue)
        {
            this.Name = venue.Name;
            this.Description = venue.Description;
            this.Location = venue.Location;
            this.Website = venue.Website;
            this.Discord = venue.Discord;
            this.Sfw = venue.Sfw;
            this.Nsfw = venue.Nsfw;
            this.Openings = venue.Openings;
            this.OpenOverrides = venue.OpenOverrides;
            this.Tags = venue.Tags;
            this.Managers = venue.Managers;
            this.Notices = venue.Notices;
            return this;
        }

        public static Venue CreateFromPublicModel(VenueModels.V2022.Venue venue, string owningKey)
        {
            var newInternalVenue = new Venue();
            newInternalVenue.Id = venue.Id;
            newInternalVenue.OwningKey = owningKey;
            newInternalVenue.UpdateFromPublicModel(venue);
            return newInternalVenue;
        }

        internal VenueModels.V2022.Venue ToPublicModel()
        {
            return new VenueModels.V2022.Venue
            {
                Added = this.Added,
                Id = this.Id,
                Description = this.Description,
                Discord = this.Discord,
                Location = this.Location,
                Managers = this.Managers,
                Name = this.Name,
                Notices = this.Notices,
                Nsfw = this.Nsfw,
                Openings = this.Openings,
                OpenOverrides = this.OpenOverrides,
                Sfw = this.Sfw,
                Tags = this.Tags,
                Website = this.Website
            };
        }
    }
}