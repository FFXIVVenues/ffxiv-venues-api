using FFXIVVenues.DomainData.Entities.Flags;

namespace FFXIVVenues.FlagService.Client.Events;

public record FlagResolvedEvent(string FlagId, ulong ResolvedBy, Flag Flag);
