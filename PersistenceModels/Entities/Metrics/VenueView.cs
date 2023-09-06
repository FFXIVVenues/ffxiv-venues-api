using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FFXIVVenues.Api.PersistenceModels.Entities.Venues;

namespace FFXIVVenues.Api.PersistenceModels.Entities;

[Table("VenueViews", Schema = "VenueMetrics")]
public class VenueView
{
    [Key] [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }
    public DateTimeOffset At { get; set; }
    public virtual Venue Venue { get; init; }

    public VenueView() =>
        At = DateTimeOffset.UtcNow;
    
    public VenueView(Venue venue) : this() =>
        Venue = venue;
}