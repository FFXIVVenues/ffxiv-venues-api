using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AutoMapper;

namespace FFXIVVenues.Api.PersistenceModels.Entities.Venues;

[Table("OpenOverrides", Schema = nameof(Entities.Venues))]
[AutoMap(typeof(VenueModels.OpenOverride), ReverseMap = true)]
public class OpenOverride
{
    [Key] [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }
    public bool Open { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    
    [Required]
    public virtual Venue Venue { get; set; }
}