using FFXIVVenues.DomainData.Entities.Flags;

namespace FFXIVVenues.FlagService.Client.Events;

public record VenueFlaggedEvent(string FlagId, string VenueId, FlagCategory Category, string? Description, string? From);
