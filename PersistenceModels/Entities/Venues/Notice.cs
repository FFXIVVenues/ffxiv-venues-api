using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AutoMapper;
using FFXIVVenues.VenueModels;

namespace FFXIVVenues.Api.PersistenceModels.Entities.Venues;

[Table("Notices", Schema = nameof(Venues))]
[AutoMap(typeof(VenueModels.Notice), ReverseMap = true)]
public class Notice
{
    [Key] [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public NoticeType Type { get; set; }
    public string Message { get; set; }
    
    [Required]
    public virtual Venue Venue { get; set; }
}