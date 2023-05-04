using System;
using FFXIVVenues.Api.InternalModel;
using Microsoft.Extensions.Configuration;

namespace FFXIVVenues.Api.Helpers;

class SplashUriBuilder : ISplashUriBuilder
{
    private readonly IConfiguration _config;

    public SplashUriBuilder(IConfiguration config)
    {
        _config = config;
    }

    public Uri GetSplashUri(Venue venue)
    {
        var template = this._config.GetValue<string>("MediaStorage:BlobUriTemplate");
        return template == null ? null : new Uri(template.Replace("{venueId}", venue.Id).Replace("{bannerKey}", venue.Banner));
    }
}