using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FFXIVVenues.Api.PersistenceModels.Entities.Venues;

[Table("Schedules", Schema = nameof(Entities.Venues))]
[PrimaryKey(nameof(VenueId), nameof(Day), nameof(StartHour), nameof(StartMinute))]
public class Schedule
{
    [ForeignKey(nameof(Venue))] protected string VenueId { get; set; }
    public Day Day { get; set; }
    public ushort StartHour { get; set; }
    public ushort StartMinute { get; set; }
    public ushort? EndHour { get; set; }
    public ushort? EndMinute { get; set; }
    public string TimeZone { get; set; }
    public virtual Location Location { get; set; }
    public virtual Venue Venue { get; set; }
}