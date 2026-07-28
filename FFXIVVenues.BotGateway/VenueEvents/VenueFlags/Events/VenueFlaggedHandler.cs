using Discord;
using Discord.WebSocket;
using FFXIVVenues.BotGateway.Api;
using FFXIVVenues.BotGateway.Infrastructure.Components;
using FFXIVVenues.BotGateway.Infrastructure.Persistence.Abstraction;
using FFXIVVenues.BotGateway.Utils;
using FFXIVVenues.BotGateway.VenueEvents.VenueFlags.Responses;
using FFXIVVenues.FlagService.Client.Events;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FFXIVVenues.BotGateway.VenueEvents.VenueFlags.Events;

public class VenueFlaggedHandler(IRepository repository, IDiscordClient client, IApiService apiService, IVenueFlagRenderer flagRenderer)
{
    public async Task Handle(VenueFlaggedEvent @event)
    {
        var streams = await repository.GetWhereAsync<EventStreamChannel>(
            i => i.EventType == StreamableEvent.Flags);
        if (!streams.Any()) 
            return;
        
        var venue = await apiService.GetVenueAsync(@event.Flag.VenueId);
        if (venue == null) return;

        var region = FfxivWorlds.GetRegionForDataCenter(venue.Location.DataCenter);
        var regionFlag = FfxivWorlds.GetFlagForRegion(region);
        var flagEmbed = flagRenderer.RenderFlag(venue, @event.Flag).Build();
        var options = flagRenderer.RenderFlagOptions(@event.Flag).Build();

        var flagDistribution = new VenueFlagDistribution(@event.FlagId, @event.Flag);
        foreach (var stream in streams)
        {
            var channel = await client.GetChannelAsync(stream.ChannelId);
            if (channel is not SocketTextChannel socketTextChannel)
            {
                Log.Debug("Channel {ChannelId} does not exist or is not a text channel, removing", stream.ChannelId);
                await repository.DeleteAsync(stream);
                continue;
            }

            try
            {
                var message = await socketTextChannel.SendMessageAsync($"{regionFlag} {region}", embeds: [ flagEmbed], components: options);
                flagDistribution.Messages.Add(new (stream.ChannelId, message.Id));
            }
            catch (Exception e)
            {
                Log.Error(e, "Could not stream event to channel {ChannelId}", stream.ChannelId);
            }
        }

        await repository.UpsertAsync(flagDistribution);
    }
}


