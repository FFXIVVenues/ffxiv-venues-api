using FFXIVVenues.BotGateway.Infrastructure.Persistence.Abstraction;
using FFXIVVenues.DomainData.Entities.Flags;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.Security;

namespace FFXIVVenues.BotGateway.VenueEvents.VenueFlags;

public record VenueFlagDistribution(string FlagId, Flag Flag) : IEntity
{
    public string id => FlagId;
    public List<FlagDistributionMessage> Messages { get; set; } = new();
}

public record FlagDistributionMessage(ulong ChannelId, ulong MessageId);
