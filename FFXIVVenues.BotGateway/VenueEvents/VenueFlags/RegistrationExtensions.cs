using FFXIVVenues.BotGateway.Infrastructure.Components;
using FFXIVVenues.BotGateway.VenueEvents.VenueFlags.Responses;

namespace FFXIVVenues.BotGateway.VenueEvents.VenueFlags;

public static class RegistrationExtensions
{

    public static T AddVenueFlagHandlers<T>(this T componentBroker) where T : IComponentBroker
    {
        if (componentBroker == null)
            return default;

        componentBroker.Add<DismissFlagHandler>(DismissFlagHandler.Key);
        componentBroker.Add<ResolveFlagHandler>(ResolveFlagHandler.Key);
        componentBroker.Add<ForwardFlagHandler>(ForwardFlagHandler.Key);

        componentBroker.Add<EditVenueFlagHandler>(EditVenueFlagHandler.Key);
        componentBroker.Add<TemporarilyCloseVenueFlagHandler>(TemporarilyCloseVenueFlagHandler.Key);
        componentBroker.Add<PermanantlyCloseVenueFlagHandler>(PermanantlyCloseVenueFlagHandler.Key);

        return componentBroker;
    }
}