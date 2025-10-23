using FFXIVVenues.VenueModels;
using System;
using FFXIVVenues.VenueModels.Observability;
using Venue = FFXIVVenues.Api.PersistenceModels.Entities.Venues.Venue;

namespace FFXIVVenues.Api.Observability
{
    public interface IChangeBroker
    {
        Action Observe(Observer observer, InvocationKind invocationKind);
        void Queue(ObservableOperation operation, Venue venue);
    }
}