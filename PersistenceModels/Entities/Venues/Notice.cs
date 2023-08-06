﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FFXIVVenues.Api.Helpers;
using FFXIVVenues.VenueModels;
using Microsoft.EntityFrameworkCore;

namespace FFXIVVenues.Api.PersistenceModels.Entities.Venues;

[Table("Notices", Schema = nameof(Venues))]
[PrimaryKey(nameof(VenueId), nameof(Id))]
public class Notice
{
    [ForeignKey(nameof(Venue))] protected string VenueId { get; set; }
    public string Id { get; set; } = IdHelper.GenerateId(3);
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public NoticeType Type { get; set; }
    public string Message { get; set; }
    
    [Required]
    public virtual Venue Venue { get; set; }
}