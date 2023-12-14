using System;
using AutoMapper;
using FFXIVVenues.VenueModels;
using Microsoft.Extensions.Configuration;

namespace FFXIVVenues.Api.PersistenceModels.Mapping;

public class MapFactory : IMapFactory
{
    private readonly MapperConfiguration _mappingConfiguration;
    private readonly MapperConfiguration _projectionConfiguration;

    public MapFactory(IConfiguration config)
    {
        var blobUriTemplate = config.GetValue<string>("MediaStorage:BlobUriTemplate");
        this._mappingConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Entities.Venues.Schedule, VenueModels.Schedule>()
                .ForMember(o => o.Start, x => x.MapFrom(s =>
                    new VenueModels.Time { Hour = s.StartHour, Minute = s.StartMinute, TimeZone = s.TimeZone }))
                .ForMember(o => o.End, x => x.MapFrom(s => s.EndHour == null? null :
                    new VenueModels.Time { Hour = s.EndHour.Value, Minute = s.EndMinute.Value, TimeZone = s.TimeZone, 
                        NextDay = s.EndHour < s.StartHour || (s.EndHour == s.StartHour && s.EndMinute < s.StartMinute )}))
                .ForMember(o => o.Interval, x => x.MapFrom(s => 
                    new Interval { IntervalType = s.IntervalType, IntervalArgument = s.IntervalArgument}));
            cfg.CreateMap<VenueModels.Schedule, Entities.Venues.Schedule>()
                .ForMember(d => d.IntervalArgument, x => x.MapFrom(s => s.Interval.IntervalArgument))
                .ForMember(d => d.IntervalType, x => x.MapFrom(s => s.Interval.IntervalType))
                .ForMember(d => d.StartHour, x => x.MapFrom(s => s.Start.Hour))
                .ForMember(d => d.StartMinute, x => x.MapFrom(s => s.Start.Minute))
                .ForMember(d => d.EndHour,  x => x.MapFrom(s => s.End == null ? (ushort?)null : s.End.Hour))
                .ForMember(d => d.EndMinute, x => x.MapFrom(s => s.End == null ? (ushort?)null : s.End.Minute))
                .ForMember(d => d.TimeZone, x => x.MapFrom(s => s.Start.TimeZone));
            cfg.CreateMap<DateTime, DateTimeOffset>().ConvertUsing(dt => new DateTimeOffset(dt.ToUniversalTime()));
            cfg.CreateMap<DateTimeOffset, DateTime>().ConvertUsing(offset => offset.UtcDateTime);
            cfg.CreateMap<Entities.Venues.ScheduleOverride, VenueModels.ScheduleOverride>().ReverseMap();
            cfg.CreateMap<Entities.Venues.Location, VenueModels.Location>().ReverseMap();
            cfg.CreateMap<Entities.Venues.Notice, VenueModels.Notice>().ReverseMap();
            cfg.CreateMap<Entities.Venues.Day, VenueModels.Day>().ReverseMap();
            cfg.CreateMap<Entities.Venues.Venue, FFXIVVenues.VenueModels.Venue>()
                .ForMember(d => d.BannerUri, o => o.MapFrom(o => new Uri(blobUriTemplate.Replace("{venueId}", o.Id).Replace("{bannerKey}", o.Banner))));
            cfg.CreateMap<VenueModels.Venue, Entities.Venues.Venue>()
                .ForMember(d => d.Added, ex => ex.Ignore())
                .ForMember(d => d.LastModified, ex => ex.Ignore())
                .ForMember(d => d.Approved, ex => ex.Ignore())
                .ForMember(d => d.ScopeKey, ex => ex.Ignore());
        });
        
        this._projectionConfiguration = new MapperConfiguration(cfg =>
        {
           cfg.CreateProjection<Entities.Venues.Venue, VenueModels.Venue>()
                .ForMember(dto => dto.BannerUri, conf => conf.MapFrom(o => 
                    o.Banner != null 
                        ? new Uri(blobUriTemplate.Replace("{venueId}", o.Id).Replace("{bannerKey}", o.Banner)) 
                        : null));
            cfg.CreateProjection<Entities.Venues.Schedule, VenueModels.Schedule>()
                .ForMember(o => o.Day, x => x.MapFrom(o => (int) o.Day))
                .ForMember(o => o.Start, x => x.MapFrom(s =>
                    new VenueModels.Time { Hour = s.StartHour, Minute = s.StartMinute, TimeZone = s.TimeZone }))
                .ForMember(o => o.End, x => x.MapFrom(s => s.EndHour == null? null :
                    new VenueModels.Time { Hour = s.EndHour.Value, Minute = s.EndMinute.Value, TimeZone = s.TimeZone, 
                        NextDay = s.EndHour < s.StartHour || (s.EndHour == s.StartHour && s.EndMinute < s.StartMinute )}))
                .ForMember(o => o.Interval, x => x.MapFrom(s => 
                    new Interval { IntervalType = s.IntervalType, IntervalArgument = s.IntervalArgument }));
            cfg.CreateProjection<Entities.Venues.Location, VenueModels.Location>();
            cfg.CreateProjection<Entities.Venues.Notice, VenueModels.Notice>();
            cfg.CreateProjection<Entities.Venues.ScheduleOverride, VenueModels.ScheduleOverride>();
        });
    }

    public IMapper GetModelMapper() =>
        this._mappingConfiguration.CreateMapper();
    
    public IMapper GetModelProjector() =>
        this._projectionConfiguration.CreateMapper();
    
}