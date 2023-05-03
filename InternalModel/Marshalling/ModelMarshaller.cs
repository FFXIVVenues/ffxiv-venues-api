using FFXIVVenues.Api.Helpers;

namespace FFXIVVenues.Api.InternalModel.Marshalling;

public class ModelMarshaller : IModelMarshaller
{
    private readonly ISplashUriBuilder _uriBuilder;

    public ModelMarshaller(ISplashUriBuilder uriBuilder)
    {
        _uriBuilder = uriBuilder;
    }

    public VenueModels.Venue MarshalAsPublicModel(Venue venue) =>
        new ()
        {
            Added = venue.Added,
            Id = venue.Id,
            Description = venue.Description,
            BannerUri = venue.Banner != null ? this._uriBuilder.GetSplashUri(venue) : null,
            Discord = venue.Discord,
            Location = venue.Location,
            Managers = venue.Managers,
            Name = venue.Name,
            Notices = venue.Notices,
            Hiring = venue.Hiring,
            Openings = venue.Openings,
            OpenOverrides = venue.OpenOverrides,
            Sfw = venue.Sfw,
            Tags = venue.Tags,
            Website = venue.Website,
            Approved = venue.Approved,
            MareCode = venue.MareCode,
            MarePassword = venue.MarePassword
        };

    public Venue MarshalAsInternalModel(in Venue venue, VenueModels.Venue publicModel)
    {
        venue.Name = publicModel.Name;
        venue.Description = publicModel.Description;
        venue.Location = publicModel.Location;
        venue.Website = publicModel.Website;
        venue.Discord = publicModel.Discord;
        venue.Sfw = publicModel.Sfw;
        venue.Hiring = publicModel.Hiring;
        venue.Openings = publicModel.Openings;
        venue.OpenOverrides = publicModel.OpenOverrides;
        venue.Tags = publicModel.Tags;
        venue.Managers = publicModel.Managers;
        venue.Notices = publicModel.Notices;
        venue.MareCode = publicModel.MareCode;
        venue.MarePassword = publicModel.MarePassword;
        return venue;
    }

    public Venue MarshalAsInternalModel(VenueModels.Venue venue, string owningKey) =>
        this.MarshalAsInternalModel(new Venue
            {
                Id = venue.Id,
                OwningKey = owningKey
            }, venue);

}