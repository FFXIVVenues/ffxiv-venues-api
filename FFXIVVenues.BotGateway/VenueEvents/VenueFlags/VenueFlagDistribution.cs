using FFXIVVenues.BotGateway.Infrastructure.Persistence.Abstraction;
using System.Collections.Generic;

namespace FFXIVVenues.BotGateway.VenueEvents.VenueFlags;

public record VenueFlagDistribution(string FlagId) : IEntity
{
    public string id => FlagId;
    public string VenueId { get; set; }
    public List<FlagDistributionMessage> Messages { get; set; } = new();
}

public record FlagDistributionMessage(ulong ChannelId, ulong MessageId);
