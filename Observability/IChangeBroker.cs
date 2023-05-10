using FFXIVVenues.VenueModels;
using System;
using FFXIVVenues.VenueModels.Observability;

namespace FFXIVVenues.Api.Observability
{
    public interface IChangeBroker
    {
        Action Observe(Observer observer, InvocationKind invocationKind);
        void Queue(ObservableOperation operation, Venue venue);
    }
}