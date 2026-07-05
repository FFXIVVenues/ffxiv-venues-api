using Discord;
using FFXIVVenues.BotGateway.Api;
using FFXIVVenues.BotGateway.Authorisation;
using FFXIVVenues.BotGateway.Infrastructure.Context;
using FFXIVVenues.BotGateway.Infrastructure.Persistence.Abstraction;
using FFXIVVenues.BotGateway.VenueAuditing.ComponentHandlers.AuditResponse;
using FFXIVVenues.FlagService.Client;
using System.Threading.Tasks;

namespace FFXIVVenues.BotGateway.VenueEvents.VenueFlags.Responses;

public class DismissFlagHandler(
    IRepository repository,
    IApiService apiService,
    IAuthorizer authorizer,
    IFlagServiceClient flagServiceClient)
    : BaseAuditHandler
{
    
    public static string Key => "DISMISS_FLAG";

    public override async Task HandleAsync(ComponentVeniInteractionContext context, string[] args)
    {
        var flagId = args[0];
        var userId = context.Interaction.User.Id;

        var flag = await repository.GetByIdAsync<VenueFlagDistribution>(flagId);
        var venue = await apiService.GetVenueAsync(flag.Flag.VenueId);

        if (!authorizer.Authorize(context.Interaction.User.Id, Permission.RespondToFlags, venue).Authorized)
        {
            await context.Interaction.Message.ReplyAsync("Sorry, I can't let you do that. 🥲", flags: MessageFlags.Ephemeral);
            return;
        }

        await flagServiceClient.ResolveFlagAsync(flagId, userId);
        await context.Interaction.FollowupAsync("Flag resolved, thankies!", ephemeral: true);
    }
    
}