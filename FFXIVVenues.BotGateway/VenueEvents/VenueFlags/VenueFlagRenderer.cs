using Discord;
using FFXIVVenues.BotGateway.Utils;
using FFXIVVenues.BotGateway.VenueAuditing;
using FFXIVVenues.BotGateway.VenueRendering;
using FFXIVVenues.DomainData.Entities.Flags;
using FFXIVVenues.VenueModels;

namespace FFXIVVenues.BotGateway.VenueEvents.VenueFlags;

public interface IVenueFlagRenderer {
    EmbedBuilder RenderFlag(Venue venue, Flag flag);
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
            .WithUrl(uiConfig.BaseUrl + "/venue/" + venue.Id)
            .WithDescription(flag.Description)
            .WithFooter(flag.SourceAddress.Substring(0, 10))
            .WithColor(Color.Red);
        return embed;
    }

}
