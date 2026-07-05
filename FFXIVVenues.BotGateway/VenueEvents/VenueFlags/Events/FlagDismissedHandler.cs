using Discord;
using Discord.WebSocket;
using FFXIVVenues.BotGateway.Api;
using FFXIVVenues.BotGateway.Infrastructure.Persistence.Abstraction;
using FFXIVVenues.FlagService.Client.Events;
using Serilog;
using System.Threading.Tasks;

namespace FFXIVVenues.BotGateway.VenueEvents.VenueFlags.Events;

public class FlagDismissedHandler(IRepository repository, IDiscordClient client, IApiService apiService, IVenueFlagRenderer flagRenderer)
{
    public async Task Handle(FlagDismissedEvent @event)
    {
        var venue = await apiService.GetVenueAsync(@event.Flag.VenueId);
        var flagDistribution = await repository.GetByIdAsync<VenueFlagDistribution>(@event.FlagId);
        var flagEmbed = flagRenderer.RenderFlag(venue, @event.Flag);
        foreach (var flagDistributionMessage in flagDistribution.Messages)
        {
            var channel = await client.GetChannelAsync(flagDistributionMessage.ChannelId);
            if (channel is not SocketTextChannel socketTextChannel)
            {
                Log.Debug("Could not update flag distribution message, channel {ChannelId} does not exist or is not a text channel, skipping", flagDistributionMessage.ChannelId);
                continue;
            }
            await socketTextChannel.ModifyMessageAsync(flagDistributionMessage.MessageId, props =>
            {
                props.Components = new ComponentBuilder().Build();
                props.Embeds = new[]
                {
                    flagEmbed.Build(),
                    new EmbedBuilder().WithDescription($"Flag dismissed by {MentionUtils.MentionUser(@event.ResolvedBy)}").Build()
                };
            });
        }
        await repository.DeleteAsync(flagDistribution);
    }
}


