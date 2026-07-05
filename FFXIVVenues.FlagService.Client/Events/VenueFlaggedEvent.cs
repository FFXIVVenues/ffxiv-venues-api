using FFXIVVenues.DomainData.Entities.Flags;

namespace FFXIVVenues.FlagService.Client.Events;

public record VenueFlaggedEvent(string FlagId, FlagCategory Category, string? Description, string? From, Flag Flag);
