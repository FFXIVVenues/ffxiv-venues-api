using System.ComponentModel.DataAnnotations.Schema;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace FFXIVVenues.Api.PersistenceModels.Entities.Venues;

[Owned]
[AutoMap(typeof(VenueModels.Time), ReverseMap = true)]
public class Time
{
    public ushort Hour { get; set; }
    public ushort Minute { get; set; }
    public string TimeZone { get; set; }
    public bool NextDay { get; set; }
}