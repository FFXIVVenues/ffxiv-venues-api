using FFXIVVenues.Api.Persistence;
using System;

namespace FFXIVVenues.Api.InternalModel
{
    public class ViewRecord : IEntity
    {
        public string Id { get => $"{What}-{When}"; set { } }
        public string OwningKey { get; set; }
        public DateTime When { get; set; }
        public string What { get; set; }
        public ViewRecord(string what)
        {
            When = DateTime.UtcNow;
            What = what;
        }
    }
}
