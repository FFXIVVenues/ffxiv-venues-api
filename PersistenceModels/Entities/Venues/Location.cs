using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FFXIVVenues.Api.Helpers;

namespace FFXIVVenues.Api.PersistenceModels.Entities.Venues;

[Table("Locations", Schema = nameof(Entities.Venues))]
public class Location
{

    [Key] public string Id { get; set; } = IdHelper.GenerateId(8);
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