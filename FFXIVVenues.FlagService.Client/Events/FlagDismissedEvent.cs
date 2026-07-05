using FFXIVVenues.DomainData.Entities.Flags;

namespace FFXIVVenues.FlagService.Client.Events;

public record FlagDismissedEvent(string FlagId, ulong ResolvedBy, Flag Flag);
