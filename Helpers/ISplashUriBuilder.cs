using System;
using FFXIVVenues.Api.InternalModel;

namespace FFXIVVenues.Api.Helpers;

public interface ISplashUriBuilder
{
    Uri GetSplashUri(Venue venue);
}