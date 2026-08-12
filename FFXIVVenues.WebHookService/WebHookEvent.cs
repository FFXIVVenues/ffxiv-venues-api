using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FFXIVVenues.WebHookService;

public record WebHookEvent<T>(
    [property:JsonPropertyName("type")] string EventType, 
    [property: JsonPropertyName("data")] T Data)
{
    [JsonPropertyName("specversion")] public string SpecVersion { get; set; } = "1.0";
    [JsonPropertyName("id")] public string Id { get; set; } = Guid.CreateVersion7().ToString();
    [JsonPropertyName("source")] public string Source { get; set; } = "https://ffxivvenues.com";
    [JsonPropertyName("time")] public DateTimeOffset EventTime { get; set; } = DateTimeOffset.UtcNow;
}

public record VenueCreatedEventData(string VenueId, VenueModels.Venue Venue);
public record VenueUpdatedEventData(string VenueId, VenueModels.Venue Venue);
public record VenueDeletedEventData(string VenueId);