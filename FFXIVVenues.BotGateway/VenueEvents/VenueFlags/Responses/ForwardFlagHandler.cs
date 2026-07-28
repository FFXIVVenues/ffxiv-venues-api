using Discord;
using Discord.WebSocket;
using FFXIVVenues.BotGateway.Api;
using FFXIVVenues.BotGateway.Authorisation;
using FFXIVVenues.BotGateway.Infrastructure.Components;
using FFXIVVenues.BotGateway.Infrastructure.Context;
using FFXIVVenues.BotGateway.Infrastructure.Persistence.Abstraction;
using FFXIVVenues.BotGateway.Utils.Broadcasting;
using FFXIVVenues.BotGateway.VenueAuditing;
using FFXIVVenues.BotGateway.VenueAuditing.ComponentHandlers.AuditResponse;
using FFXIVVenues.BotGateway.VenueEvents.VenueFlags.Events;
using FFXIVVenues.FlagService.Client;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FFXIVVenues.BotGateway.VenueEvents.VenueFlags.Responses;

public class ForwardFlagHandler(IVenueFlagRenderer flagRenderer, IDiscordClient discordClient, IRepository repository, IApiService apiService, IAuthorizer authorizer) : IComponentHandler
{
    public static string Key => "FLAG_RESPONSE_FORWARD";

    public async Task HandleAsync(ComponentVeniInteractionContext context, string[] args)
    {
        var flagId = args[0];
        var userId = context.Interaction.User.Id;

        var flag = await repository.GetByIdAsync<VenueFlagDistribution>(flagId);
        var venue = await apiService.GetVenueAsync(flag.Flag.VenueId);

        if (!authorizer.Authorize(context.Interaction.User.Id, Permission.RespondToFlags, venue).Authorized)
        {
            await context.Interaction.Message.ReplyAsync(FlagStrings.NoPermission, flags: MessageFlags.Ephemeral);
            return;
        }

        var flagEmbed = flagRenderer.RenderFlag(venue, flag.Flag);

        var broadcast = new Broadcast(Guid.NewGuid().ToString(), discordClient)
               .WithMessage(FlagStrings.FlagReceived)
               .WithEmbed(flagEmbed)
               .WithComponent(ctx => new ComponentBuilder()
                   .WithSelectMenu(new SelectMenuBuilder()
                       .WithValueHandlers()
                       .WithPlaceholder("Select response")
                       .AddOption(new SelectMenuOptionBuilder()
                           .WithLabel("Dismiss Flag")
                           .WithEmote(new Emoji("❎"))
                           .WithDescription("Dismiss the flag as needing no action.")
                           .WithStaticHandler(DismissFlagHandler.Key, flag.id))
                       .AddOption(new SelectMenuOptionBuilder()
                           .WithLabel("Edit Venue")
                           .WithEmote(new Emoji("✏️"))
                           .WithDescription("Update the details on this venue.")
                           .WithStaticHandler(EditVenueFlagHandler.Key, flag.id))
                       .AddOption(new SelectMenuOptionBuilder()
                           .WithLabel("Close Venue")
                           .WithEmote(new Emoji("🔒"))
                           .WithDescription("Put this venue on a hiatus for up to 3 months.")
                           .WithStaticHandler(TemporarilyCloseVenueFlagHandler.Key, flag.id))
                       .AddOption(new SelectMenuOptionBuilder()
                            .WithLabel("Permanently Close / Delete")
                            .WithEmote(new Emoji("❌"))
                            .WithDescription("Delete this venue completely.")
                            .WithStaticHandler(PermanentlyCloseVenueFlagHandler.Key, flag.id))
                       ));
        var broadcastReceipt = await broadcast.SendToAsync(venue.Managers.Select(ulong.Parse).ToArray());
        var successful = broadcastReceipt.BroadcastMessages.Where(m => m.Status == MessageStatus.Sent).Select(b => b.UserId);

        var flagOptionsComponent = flagRenderer.RenderFlagOptions(flag.Flag, RenderOptions.ResolveDismiss).Build();

        if (!successful.Any())
        {
            await context.Interaction.FollowupAsync("I couldn't forward the flag to any managers. 🥲", flags: MessageFlags.Ephemeral);
            return;
        }

        await context.Interaction.FollowupAsync($"I've forwarded the flag to {string.Join(",", successful.Select(MentionUtils.MentionUser))}! 😊", flags: MessageFlags.Ephemeral);
        foreach (var message in flag.Messages)
        {
            var channel = await discordClient.GetChannelAsync(message.ChannelId);
            if (channel is not SocketTextChannel socketTextChannel)
            {
                Log.Debug("Could not update flag distribution message, channel {ChannelId} does not exist or is not a text channel, skipping", message.ChannelId);
                continue;
            }

            await socketTextChannel.ModifyMessageAsync(message.MessageId, props =>
            {
                props.Embeds = new[]
                {
                    flagEmbed.Build(),
                    new EmbedBuilder().WithDescription($"{MentionUtils.MentionUser(userId)} forwarded the flag").Build(),
                };
                props.Components = flagOptionsComponent;
            });
        }

        flag.Messages.AddRange(broadcastReceipt.BroadcastMessages.Select(m => new FlagDistributionMessage(m.ChannelId, m.MessageId)));
        await repository.UpsertAsync(flag);
    }
}