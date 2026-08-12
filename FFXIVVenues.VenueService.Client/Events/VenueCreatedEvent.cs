using System;

namespace FFXIVVenues.VenueService.Client.Events;

public record VenueCreatedEvent(string VenueId) 
{

    public DateTimeOffset At { get; set; } = DateTimeOffset.UtcNow;

}
