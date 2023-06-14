using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AutoMapper;
using AutoMapper.Configuration.Annotations;
using FFXIVVenues.Api.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FFXIVVenues.Api.PersistenceModels.Entities.Venues;

[Table("Venues", Schema = nameof(Entities.Venues))]
[AutoMap(typeof(VenueModels.Venue), ReverseMap = true)]
public class Venue : IEntity, ISecurityScoped
{
    [Key] public string Id { get; init; }
    public string Name { get; set; }
    public string Banner { get; set; }
    [Ignore] public DateTime Added { get; set; }
    [Ignore] public DateTime? LastModified { get; set; }
    public virtual List<string> Description { get; set; } = new ();
    public virtual Location Location { get; set; } = new ();
    public Uri Website { get; set; }
    public Uri Discord { get; set; }
    public bool Hiring { get; set; }
    public bool Sfw { get; set; }
    [DeleteBehavior(DeleteBehavior.Cascade)] public virtual List<Opening> Openings { get; set; } = new ();
    [DeleteBehavior(DeleteBehavior.Cascade)] public virtual List<OpenOverride> OpenOverrides { get; set; } = new ();
    [DeleteBehavior(DeleteBehavior.Cascade)] public virtual List<Notice> Notices { get; set; } = new ();
    public virtual List<string> Managers { get; set; } = new ();
    public virtual List<string> Tags { get; set; } = new ();
    public DateTime HiddenUntil { get; set; }
    public string MareCode { get; set; }
    public string MarePassword { get; set; }
    [Ignore] public bool Approved { get; set; }
    [Ignore] public string ScopeKey { get; set; }

    public Venue()
    {
        this.Added = DateTime.UtcNow;
    }
    
}