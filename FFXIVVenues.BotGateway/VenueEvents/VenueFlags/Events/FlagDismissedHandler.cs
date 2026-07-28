using Discord;
using Discord.WebSocket;
using FFXIVVenues.BotGateway.Api;
using FFXIVVenues.BotGateway.Infrastructure.Persistence.Abstraction;
using FFXIVVenues.FlagService.Client.Events;
using Serilog;
using System.Linq;
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
            if (channel is SocketTextChannel socketTextChannel)
            {
                await socketTextChannel.ModifyMessageAsync(flagDistributionMessage.MessageId, props =>
                {
                    props.Components = new ComponentBuilder().Build();
                    props.Embeds = new[]
                    {
                        flagEmbed.Build(),
                        new EmbedBuilder().WithDescription($"{MentionUtils.MentionUser(@event.DismissedBy)} dismissed the flag").Build()
                    };
                });
            }
            else if (channel is SocketDMChannel socketDMChannel)
            {
                var isResolver = socketDMChannel.Users.Any(u => u.Id == @event.DismissedBy);
                var message = $"❎ You dismissed the flag";
                if (!isResolver)
                    message = $"❎ {MentionUtils.MentionUser(@event.DismissedBy)} dismissed the flag";

                await socketDMChannel.ModifyMessageAsync(flagDistributionMessage.MessageId, props =>
                {
                    props.Components = new ComponentBuilder().Build();
                    props.Embeds = new[]
                    {
                        flagEmbed.Build(),
                        new EmbedBuilder().WithDescription(message).Build()
                    };
                });
            }
            else
            {
                Log.Debug("Could not update flag distribution message, channel {ChannelId} does not exist or is not a text channel, skipping", flagDistributionMessage.ChannelId);
                continue;
            }
        }
        await repository.DeleteAsync(flagDistribution);
    }
}


