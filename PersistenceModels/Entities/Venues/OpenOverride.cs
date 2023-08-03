using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FFXIVVenues.Api.PersistenceModels.Entities.Venues;

[Table("OpenOverrides", Schema = nameof(Venues))]
[PrimaryKey(nameof(VenueId), nameof(Start))]
public class OpenOverride
{
    [ForeignKey(nameof(Venue))] protected string VenueId { get; set; }
    public bool Open { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    
    [Required]
    public virtual Venue Venue { get; set; }
}