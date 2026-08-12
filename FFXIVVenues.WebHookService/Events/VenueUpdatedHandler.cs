using AutoMapper;
using FFXIVVenues.DomainData.Context;
using FFXIVVenues.DomainData.Mapping;
using FFXIVVenues.VenueService.Client.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFXIVVenues.WebHookService.Events;

public class VenueUpdatedHandler(WebHookEventDistributor webHookDispatcher, DomainDataContext domainData, IMapFactory mapFactory)
{
    private readonly IMapper _modelMapper = mapFactory.GetModelMapper();

    public Task Handle(VenueUpdatedEvent @event)
    {
        Log.Information("Received Venue Updated event: {event}", @event);

        var venue = domainData.Venues.Find(@event.VenueId);
        if (venue is null)
        {
            Log.Information("Did not emit WebHook event for venue updated event: {event}; venue with the given id did not exist", @event);
            return Task.CompletedTask;
        }

        var data = new VenueUpdatedEventData(venue.Id, this._modelMapper.Map<VenueModels.Venue>(venue));
        webHookDispatcher.DispatchEvent("com.ffxivvenues.venue.updated", data);
        return Task.CompletedTask;
    }
}
