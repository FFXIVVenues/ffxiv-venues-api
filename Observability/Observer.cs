using FFXIVVenues.Api.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FFXIVVenues.VenueModels.Observability;

namespace FFXIVVenues.Api.Observability
{
    public class Observer : ObserveRequest
    {
        public string Id { get; set; } = IdHelper.GenerateId(8);

        public Func<ObservableOperation, VenueModels.Venue, Task> ObserverAction { get; set; }

        public Observer(IEnumerable<ObservableOperation> operationCriteria,
            ObservableKey? keyCriteria, string valueCriteria) 
            : base(operationCriteria, keyCriteria, valueCriteria)
        {  }

    }
}