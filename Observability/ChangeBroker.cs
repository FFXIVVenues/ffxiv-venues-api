using FFXIVVenues.VenueModels.V2022;
using System;
using System.Collections.Generic;

namespace FFXIVVenues.Api.Observability
{
    public class ChangeBroker : IChangeBroker
    {

        private IList<Observer> _observers = new List<Observer>();

        public Action Observe(Observer observer)
        {
            _observers.Add(observer);
            return () =>
            {
                lock (_observers) {
                    _observers.Remove(observer);
                }
            };
        }

        public void Invoke(ObservableOperation operation, Venue venue)
        {
            lock (this._observers)
                foreach (var observer in _observers)
                    if (observer.Matches(operation, venue))
                        observer.ObserverAction(operation, venue);
        }

    }
}
