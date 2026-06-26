using Discord;
using FFXIVVenues.BotGateway.Api;
using FFXIVVenues.BotGateway.Authorisation;
using FFXIVVenues.BotGateway.Infrastructure.Components;
using FFXIVVenues.BotGateway.Infrastructure.Context;
using FFXIVVenues.BotGateway.Infrastructure.Persistence.Abstraction;
using FFXIVVenues.FlagService.Client;
using System.Threading.Tasks;

namespace FFXIVVenues.BotGateway.VenueEvents.VenueFlags.Responses;

public class ResolveFlagHandler(IFlagServiceClient flagServiceClient, IRepository repository, IApiService apiService, IAuthorizer authorizer) : IComponentHandler
{
    public static string Key => "RESOLVE_FLAG";


    public async Task HandleAsync(ComponentVeniInteractionContext context, string[] args)
    {
        var flagId = args[0];
        var userId = context.Interaction.User.Id;

        var flag = await repository.GetByIdAsync<VenueFlagDistribution>(flagId);
        var venue = await apiService.GetVenueAsync(flag.VenueId);

        if (!authorizer.Authorize(context.Interaction.User.Id, Permission.RespondToFlags, venue).Authorized)
        {
            await context.Interaction.Message.ReplyAsync("Sorry, I can't let you do that. 🥲", flags: MessageFlags.Ephemeral);
            return;
        }

        await flagServiceClient.ResolveFlagAsync(flagId, userId);
        await context.Interaction.RespondAsync("Flag resolved, thankies!", ephemeral: true);
    }
}