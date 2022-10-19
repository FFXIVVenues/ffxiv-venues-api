using FFXIVVenues.Api.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FFXIVVenues.Api.Observability
{
    public class Observer
    {
        public string Id { get; set; } = IdHelper.GenerateId(8);

        public ObservableKey? KeyCriteria { get; set; }

        public string ValueCriteria { get; set; }

        public IEnumerable<ObservableOperation> OperationCriteria { get; set; }

        public Action<ObservableOperation, VenueModels.Venue> ObserverAction { get; internal set; }

        public bool Matches(ObservableOperation operation, VenueModels.Venue venue)
        {
            if (venue == null) return false;
            if (!OperationCriteria.Contains(operation)) return false;
            return KeyCriteria switch
            {
                ObservableKey.Id => venue.Id == ValueCriteria,
                ObservableKey.DataCenter => venue.Location.DataCenter == ValueCriteria,
                ObservableKey.World => venue.Location.World == ValueCriteria,
                ObservableKey.Manager => venue.Managers.Contains(ValueCriteria),
                _ => true,
            };
        }

    }
}