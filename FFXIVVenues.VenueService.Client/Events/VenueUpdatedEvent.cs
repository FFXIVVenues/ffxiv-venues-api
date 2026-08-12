using System;

namespace FFXIVVenues.VenueService.Client.Events;

public record VenueUpdatedEvent(string VenueId) 
{

    public DateTimeOffset At { get; set; } = DateTimeOffset.UtcNow;

}
