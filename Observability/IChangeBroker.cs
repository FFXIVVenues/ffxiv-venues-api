using FFXIVVenues.VenueModels.V2022;
using System;

namespace FFXIVVenues.Api.Observability
{
    public interface IChangeBroker
    {
        Action Observe(Observer observer);
        void Invoke(ObservableOperation operation, Venue venue);
    }
}