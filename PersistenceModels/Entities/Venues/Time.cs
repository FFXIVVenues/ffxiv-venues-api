using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace FFXIVVenues.Api.PersistenceModels.Entities.Venues;

[Owned]
public class Time
{
    public ushort Hour { get; set; }
    public ushort Minute { get; set; }
    public string TimeZone { get; set; }
    public bool NextDay { get; set; }
}