using Discord;
using FFXIVVenues.BotGateway.Infrastructure.Components;
using FFXIVVenues.BotGateway.VenueEvents.VenueFlags.Responses;
using FFXIVVenues.BotGateway.VenueRendering;
using FFXIVVenues.DomainData.Entities.Flags;
using FFXIVVenues.VenueModels;
using JasperFx.Events;
using System;

namespace FFXIVVenues.BotGateway.VenueEvents.VenueFlags;

public interface IVenueFlagRenderer {
    EmbedBuilder RenderFlag(Venue venue, Flag flag);
    ComponentBuilder RenderFlagOptions(Flag flag, RenderOptions options = RenderOptions.All);
}

public class VenueFlagRenderer(UiConfiguration uiConfig) : IVenueFlagRenderer
{
    public EmbedBuilder RenderFlag(Venue venue, Flag flag)
    {
        var embed = new EmbedBuilder()
            .WithAuthor("Venue Flagged: " +
                flag.Category switch {
                    FlagCategory.VenueEmpty => "Venue empty",
                    FlagCategory.InappropriateContent => "Inappropriate content",
                    FlagCategory.IncorrectInformation => "Site information incorrect",
                    _ => flag.Category })
            .WithTitle(venue.Name)
            .WithTimestamp(flag.Timestamp)
            .WithUrl(uiConfig.BaseUrl + "/venue/" + venue.Id)
            .WithDescription(flag.Description)
            .WithFooter(flag.SourceAddress.Substring(0, 10))
            .WithColor(Color.Red);
        return embed;
    }

    public ComponentBuilder RenderFlagOptions(Flag flag, RenderOptions options = RenderOptions.All)
    {
        var builder = new ComponentBuilder();
        var dropDown = new SelectMenuBuilder()
            .WithValueHandlers()
            .WithPlaceholder("What would you like to do?");

        if (options.HasFlag(RenderOptions.Forward))
            dropDown.AddOption(new SelectMenuOptionBuilder()
                .WithLabel("Forward")
                .WithEmote(new Emoji("⏩"))
                .WithDescription("Forward the flag to venue owners/managers for actioning.")
                .WithStaticHandler(ForwardFlagHandler.Key, flag.Id));

        if (options.HasFlag(RenderOptions.Resolve))
            dropDown.AddOption(new SelectMenuOptionBuilder()
                .WithLabel("Resolve Flag")
                .WithEmote(new Emoji("✅"))
                .WithDescription("The flag has been handled with corrective actions.")
                .WithStaticHandler(ResolveFlagHandler.Key, flag.Id));

        if (options.HasFlag(RenderOptions.Dismiss))
            dropDown.AddOption(new SelectMenuOptionBuilder()
                .WithLabel("Dismiss Flag")
                .WithEmote(new Emoji("❎"))
                .WithDescription("The flag needs no action.")
                .WithStaticHandler(DismissFlagHandler.Key, flag.Id));

        builder.WithSelectMenu(dropDown);
        return builder;
    }

}

[Flags]
public enum RenderOptions
{
    None = 0,
    Forward = 1,
    Resolve = 2, 
    Dismiss = 4,
    ResolveDismiss = Resolve | Dismiss,
    All = Forward | Resolve | Dismiss
}