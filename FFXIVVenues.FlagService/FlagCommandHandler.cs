using System.Security.Cryptography;
using System.Text;
using FFXIVVenues.DomainData.Context;
using FFXIVVenues.DomainData.Entities.Flags;
using FFXIVVenues.FlagService.Client.Commands;
using FFXIVVenues.FlagService.Client.Events;
using Wolverine;

namespace FFXIVVenues.FlagService;

public class FlagCommandHandler(IMessageBus bus, DomainDataContext domainData, ILogger<FlagCommandHandler> logger)
{
    public ValueTask Handle(FlagVenueCommand command)
    {
        var sourceAddress = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(command.SourceAddress)));
        logger.LogInformation("Handling flag for venue {VenueId} from {SourceAddress}", command.VenueId, sourceAddress);
        
        var venueExists = domainData.Venues.Any(v => v.Id == command.VenueId && v.Deleted == null);
        if (!venueExists)
        {
            logger.LogInformation("Rejecting flag from {SourceAddress} for venue {VenueId}, venue does not exist", sourceAddress, command.VenueId);   
            return ValueTask.CompletedTask;
        }
        
        var recentFlagsFromAddress = domainData.Flags.Any(f => 
            f.SourceAddress == sourceAddress && 
            f.Timestamp > DateTimeOffset.UtcNow.AddMinutes(-1)); 
        if (recentFlagsFromAddress) 
        {
            logger.LogInformation("Rejecting flag, {SourceAddress} flagged in last 1 minutes", sourceAddress);
            return ValueTask.CompletedTask;
        }
        
        var recentFlagsForVenueFromAddress = domainData.Flags.Any(f => 
            f.VenueId == command.VenueId &&
            f.SourceAddress == sourceAddress && 
            f.Category == command.Category &&
            f.Timestamp > DateTimeOffset.UtcNow.AddHours(-20));
        if (recentFlagsForVenueFromAddress)
        {
            logger.LogInformation("Rejecting flag, {SourceAddress} flagged venue {VenueId} in last 20 hours", sourceAddress, command.VenueId);
            return ValueTask.CompletedTask;
        }

        var flag = new Flag
        {
            VenueId = command.VenueId,
            SourceAddress = sourceAddress,
            Category = command.Category,
            Description = command.Description
        };
        logger.LogDebug("Flag for venue {VenueId} from {SourceAddress} accepted, saving", command.VenueId, sourceAddress);
        domainData.Flags.Add(flag);
        domainData.SaveChanges();
        logger.LogInformation("Flag {FlagId} for venue {VenueId} from {SourceAddress} saved", flag.Id, command.VenueId, sourceAddress);

        logger.LogDebug("Emitting venue flagged event for flag {FlagId} for venue {VenueId}", flag.Id, command.VenueId);
        return bus.PublishAsync(new VenueFlaggedEvent(
            flag.Id,
            command.Category,
            command.Description,
            sourceAddress,
            flag
        ));
    }
}
