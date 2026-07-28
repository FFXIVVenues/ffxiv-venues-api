using Discord;
using Discord.Rest;
using Discord.WebSocket;
using FFXIVVenues.BotGateway.Api;
using FFXIVVenues.BotGateway.Infrastructure.Persistence.Abstraction;
using FFXIVVenues.FlagService.Client.Events;
using Serilog;
using System.Linq;
using System.Threading.Tasks;

namespace FFXIVVenues.BotGateway.VenueEvents.VenueFlags.Events;

public class FlagResolvedHandler(IRepository repository, IApiService apiService, IDiscordClient client, IVenueFlagRenderer flagRenderer)
{
    public async Task Handle(FlagResolvedEvent @event)
    {
        var venue = await apiService.GetVenueAsync(@event.Flag.VenueId);
        var flagDistribution = await repository.GetByIdAsync<VenueFlagDistribution>(@event.FlagId);
        var flagEmbed = flagRenderer.RenderFlag(venue, @event.Flag);
        foreach (var flagDistributionMessage in flagDistribution.Messages)
        {
            var channel = await client.GetChannelAsync(flagDistributionMessage.ChannelId);
            if (channel is SocketTextChannel textChannel)
            {
                await textChannel.ModifyMessageAsync(flagDistributionMessage.MessageId, props =>
                {
                    props.Components = new ComponentBuilder().Build();
                    props.Embeds = new[]
                    {
                    flagEmbed.Build(),
                    new EmbedBuilder().WithDescription($"{MentionUtils.MentionUser(@event.ResolvedBy)} resolved the flag").Build()
                };
                });
            }
            else if (channel is RestDMChannel dmChannel)
            {
                var isResolver = dmChannel.Users.Any(u => u.Id == @event.ResolvedBy);
                var message = $"✅ You resolved the flag";
                if (!isResolver)
                    message = $"✅ {MentionUtils.MentionUser(@event.ResolvedBy)} resolved the flag";

                await dmChannel.ModifyMessageAsync(flagDistributionMessage.MessageId, props =>
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


