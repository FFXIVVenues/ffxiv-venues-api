using System;
using System.Security.Policy;
using AutoMapper;
using FFXIVVenues.Api.PersistenceModels.Entities.Venues;
using Microsoft.Extensions.Configuration;

namespace FFXIVVenues.Api.PersistenceModels.Mapping;

public interface ISplashUriMapper : IValueResolver<Venue, VenueModels.Venue, Uri> { }

public class SplashUriMapper : ISplashUriMapper
{
    private readonly IConfiguration _config;

    public SplashUriMapper(IConfiguration config) =>
        _config = config;
    
    public Uri Resolve(Venue source, VenueModels.Venue destination, Uri destMember, ResolutionContext context)
    {
        var template = this._config.GetValue<string>("MediaStorage:BlobUriTemplate");
        return template == null ? null : new Uri(template.Replace("{venueId}", source.Id).Replace("{bannerKey}", source.Banner));
    }
}

//==========