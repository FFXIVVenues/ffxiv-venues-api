using AutoMapper;
using FFXIVVenues.DomainData.Context;
using FFXIVVenues.DomainData.Mapping;
using FFXIVVenues.VenueService.Client.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFXIVVenues.WebHookService.Events;

public class VenueCreatedHandler(DomainDataContext domainData, IMapFactory mapFactory, WebHookEventDistributor webHookDispatcher)
{
    private readonly IMapper _modelMapper = mapFactory.GetModelMapper();

    public Task Handle(VenueCreatedEvent @event)
    {
        Log.Information("Received Venue Created event: {event}", @event);

        var venue = domainData.Venues.Find(@event.VenueId);
        if (venue is null)
        {
            Log.Information("Did not emit WebHook event for venue created event: {event}; venue with the given id did not exist", @event);
            return Task.CompletedTask;
        }

        var data = new VenueCreatedEventData(venue.Id, this._modelMapper.Map<VenueModels.Venue>(venue));
        webHookDispatcher.DispatchEvent("com.ffxivvenues.venue.created", data);
        return Task.CompletedTask;
    }

}
