using FFXIVVenues.Api.Persistence;
using System;

namespace FFXIVVenues.Api.InternalModel
{
    public class Venue : VenueModels.Venue, IEntity
    {

        public string OwningKey { get; set; }
        public string Banner { get; set; }
        public DateTime HiddenUntil { get; set; }

        public Venue UpdateFromPublicModel(VenueModels.Venue venue)
        {
            this.Name = venue.Name;
            this.Description = venue.Description;
            this.Location = venue.Location;
            this.Website = venue.Website;
            this.Discord = venue.Discord;
            this.Sfw = venue.Sfw;
            this.Hiring = venue.Hiring;
            this.Openings = venue.Openings;
            this.OpenOverrides = venue.OpenOverrides;
            this.Tags = venue.Tags;
            this.Managers = venue.Managers;
            this.Notices = venue.Notices;
            this.MareCode = venue.MareCode;
            this.MarePassword = venue.MarePassword;
            return this;
        }

        public static Venue CreateFromPublicModel(VenueModels.Venue venue, string owningKey)
        {
            var newInternalVenue = new Venue();
            newInternalVenue.Id = venue.Id;
            newInternalVenue.OwningKey = owningKey;
            newInternalVenue.UpdateFromPublicModel(venue);
            return newInternalVenue;
        }

        internal VenueModels.Venue ToPublicModel(IMediaRepository mediaRepo)
        {
            return new VenueModels.Venue
            {
                Added = this.Added,
                Id = this.Id,
                Description = this.Description,
                BannerUri = this.Banner != null ? mediaRepo.GetUri(this.Banner) : null,
                Discord = this.Discord,
                Location = this.Location,
                Managers = this.Managers,
                Name = this.Name,
                Notices = this.Notices,
                Hiring = this.Hiring,
                Openings = this.Openings,
                OpenOverrides = this.OpenOverrides,
                Sfw = this.Sfw,
                Tags = this.Tags,
                Website = this.Website,
                Approved = this.Approved,
                MareCode = this.MareCode,
                MarePassword = this.MarePassword
            };
        }
    }
}