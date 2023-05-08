using System;
using FFXIVVenues.Api.InternalModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;

namespace FFXIVVenues.Api.Helpers;

class SplashUriBuilder : ISplashUriBuilder
{
    private readonly IConfiguration _config;
    private readonly IHttpContextAccessor _contextAccessor;

    public SplashUriBuilder(IConfiguration config, IHttpContextAccessor contextAccessor)
    {
        _config = config;
        _contextAccessor = contextAccessor;
    }

    public Uri GetSplashUri(Venue venue)
    {
        var template = this._config.GetValue<string>("MediaStorage:BlobUriTemplate");
        return template == null ? null : new Uri(template.Replace("{venueId}", venue.Id).Replace("{bannerKey}", venue.Banner));
    }
}