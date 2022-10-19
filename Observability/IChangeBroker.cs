using FFXIVVenues.VenueModels;
using System;

namespace FFXIVVenues.Api.Observability
{
    public interface IChangeBroker
    {
        Action Observe(Observer observer);
        void Invoke(ObservableOperation operation, Venue venue);
    }
}