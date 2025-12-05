using FFXIVVenues.FlagService.Client.Commands;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;
using Wolverine.Pulsar;

namespace FFXIVVenues.FlagService.Client;

public static class BuilderExtensions
{
    public static IServiceCollection AddFlagService(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IFlagServiceClient, FlagServiceClient>();
        return serviceCollection;
    }
    
    public static WolverineOptions AddFlagServiceMessages(this WolverineOptions wolverineOptions)
    {
        wolverineOptions.PublishMessage<FlagVenueCommand>()
            .ToPulsarTopic("persistent://ffxivvenues/flagging/commands");
        return wolverineOptions;
    }
}