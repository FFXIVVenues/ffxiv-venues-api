using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AutoMapper;

namespace FFXIVVenues.Api.PersistenceModels.Entities.Venues;

[Table("Locations", Schema = nameof(Entities.Venues))]
[AutoMap(typeof(VenueModels.Location), ReverseMap = true)]
public class Location
{
    [Key] [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    
    public string Id { get; set; }
    
    [Index("Address")] public string DataCenter { get; set; }
    [Index("Address")] public string World { get; set; }
    [Index("Address")] public string District { get; set; }
    [Index("Address")] public ushort Ward { get; set; }
    [Index("Address")] public ushort Plot { get; set; }
    [Index("Address")] public ushort Apartment { get; set; }
    [Index("Address")] public ushort Room { get; set; }
    [Index("Address")] public bool Subdivision { get; set; }
    [Index("Override")] public string Override { get; set; }
    
    public virtual List<Venue> Venues { get; set; }

}