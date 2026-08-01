using System.Threading.Tasks;
using Discord;
using FFXIVVenues.BotGateway.Infrastructure.Context;
using FFXIVVenues.BotGateway.Infrastructure.Context.SessionHandling;
using FFXIVVenues.BotGateway.Utils;
using FFXIVVenues.BotGateway.VenueControl;
using FFXIVVenues.BotGateway.VenueControl.VenueAuthoring;
using FFXIVVenues.BotGateway.VenueControl.VenueAuthoring.PropertyEntrySessionStates.LocationEntry;

namespace FFXIVVenues.BotGateway.VenueControl.VenueAuthoring.PropertyEntrySessionStates;

class DescriptionEntrySessionState : ISessionState
{
    public Task Enter(VeniInteractionContext c)
    {
        c.Session.RegisterMessageHandler(this.OnMessageReceived);
        var isDm = c.Interaction.Channel is IDMChannel;

        var message = VenueControlStrings.AskForDescriptionMessage;
        if (!isDm)
            message += "\n-# " + VenueControlStrings.AtVeniWithAnswerMessage;

        return c.Interaction.RespondAsync(message,
            new ComponentBuilder()
                .WithBackButton(c)
                .WithSkipButton<LocationTypeEntrySessionState, ConfirmVenueSessionState>(c)
                .Build());
    }

    public Task OnMessageReceived(MessageVeniInteractionContext c)
    {
        var venue = c.Session.GetVenue();
        venue.Description = c.Interaction.Content.StripMentions().AsListOfParagraphs();
        if (c.Session.InEditing())
            return c.Session.MoveStateAsync<ConfirmVenueSessionState>(c);
        return c.Session.MoveStateAsync<LocationTypeEntrySessionState>(c);
    }

}