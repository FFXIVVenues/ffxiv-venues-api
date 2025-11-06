using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FFXIVVenues.DomainData.Entities.Venues;

namespace FFXIVVenues.DomainData.Entities.Metrics;

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