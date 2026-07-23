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
        var text = $"Flag received for a venue in {regionFlag} {region}";
        var flagEmbed = flagRenderer.RenderFlag(venue, @event.Flag).Build();
        
        var builder = new ComponentBuilder();
        var dropDown = new SelectMenuBuilder()
            .WithValueHandlers()
            .WithPlaceholder("What would you like to do?");
        
        dropDown.AddOption(new SelectMenuOptionBuilder()
            .WithLabel("Resolve")
            .WithEmote(new Emoji("✅"))
            .WithDescription("The flag has been handled with corrective actions.")
            .WithStaticHandler(ResolveFlagHandler.Key, @event.FlagId));

        dropDown.AddOption(new SelectMenuOptionBuilder()
            .WithLabel("Dismiss")
            .WithEmote(new Emoji("❌"))
            .WithDescription("The flag needs no action.")
            .WithStaticHandler(DismissFlagHandler.Key, @event.FlagId));

        builder.WithSelectMenu(dropDown);
        
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
                var message = await socketTextChannel.SendMessageAsync(text, embeds: [ flagEmbed ], components: builder.Build());
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


