using FFXIVVenues.DomainData.Entities.Flags;

namespace FFXIVVenues.FlagService.Client.Events;

public record FlagDismissedEvent(string FlagId, ulong DismissedBy, Flag Flag);
