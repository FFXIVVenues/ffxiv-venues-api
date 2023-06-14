using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AutoMapper;

namespace FFXIVVenues.Api.PersistenceModels.Entities.Venues;

[Table("Openings", Schema = nameof(Entities.Venues))]
[AutoMap(typeof(VenueModels.Opening), ReverseMap = true)]
public class Opening
{
    [Key] [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }
    public Day Day { get; set; }
    public Time Start { get; set; }
    public Time End { get; set; }
    public virtual Location Location { get; set; }
    
    [Required]
    public virtual Venue Venue { get; set; }
}