using FFXIVVenues.VenueService.Client.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace FFXIVVenues.WebHookService.Events;

public class VenueDeletedHandler(WebHookEventDistributor webHookDispatcher)
{
    public Task Handle(VenueDeletedEvent @event)
    {
        Log.Information("Received Venue Deleted event: {event}", @event);

        var data = new VenueDeletedEventData(@event.VenueId);
        webHookDispatcher.DispatchEvent("com.ffxivvenues.venue.deleted", data);
        return Task.CompletedTask;
    }
}
