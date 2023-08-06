using System;
using AutoMapper;
using FFXIVVenues.Api.PersistenceModels.Entities.Venues;
using Microsoft.Extensions.Configuration;

namespace FFXIVVenues.Api.PersistenceModels.Mapping;

public static class AutoMapping
{

    public static IMapper GetModelMapper(IConfiguration config)
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            var blobUriTemplate = config.GetValue<string>("MediaStorage:BlobUriTemplate");
            cfg.CreateMap<Opening, VenueModels.Opening>()
                .ForMember(o => o.Start, x => x.MapFrom(s =>
                    new VenueModels.Time { Hour = s.StartHour, Minute = s.StartMinute, TimeZone = s.TimeZone }))
                .ForMember(o => o.End, x => x.MapFrom(s => s.EndHour == null? null :
                    new VenueModels.Time { Hour = s.EndHour.Value, Minute = s.EndMinute.Value, TimeZone = s.TimeZone, 
                        NextDay = s.EndHour < s.StartHour || (s.EndHour == s.StartHour && s.EndMinute < s.StartMinute )}));
            cfg.CreateMap<VenueModels.Opening, Opening>()
                .ForMember(d => d.StartHour, x => x.MapFrom(s => s.Start.Hour))
                .ForMember(d => d.StartMinute, x => x.MapFrom(s => s.Start.Minute))
                .ForMember(d => d.EndHour,  x => x.MapFrom(s => s.End == null ? (ushort?)null : s.End.Hour))
                .ForMember(d => d.EndMinute, x => x.MapFrom(s => s.End == null ? (ushort?)null : s.End.Minute))
                .ForMember(d => d.TimeZone, x => x.MapFrom(s => s.Start.TimeZone));
            cfg.CreateMap<DateTime, DateTimeOffset>().ConvertUsing(dt => new DateTimeOffset(dt.ToUniversalTime()));
            cfg.CreateMap<DateTimeOffset, DateTime>().ConvertUsing(offset => offset.UtcDateTime);
            cfg.CreateMap<OpenOverride, VenueModels.OpenOverride>().ReverseMap();
            cfg.CreateMap<Location, VenueModels.Location>().ReverseMap();
            cfg.CreateMap<Notice, VenueModels.Notice>().ReverseMap();
            cfg.CreateMap<Day, VenueModels.Day>().ReverseMap();
            cfg.CreateMap<Venue, FFXIVVenues.VenueModels.Venue>()
                .ForMember(d => d.BannerUri, o => o.MapFrom(o => new Uri(blobUriTemplate.Replace("{venueId}", o.Id).Replace("{bannerKey}", o.Banner))));
            cfg.CreateMap<FFXIVVenues.VenueModels.Venue, Venue>()
                .ForMember(d => d.Added, ex => ex.Ignore())
                .ForMember(d => d.LastModified, ex => ex.Ignore())
                .ForMember(d => d.Approved, ex => ex.Ignore())
                .ForMember(d => d.ScopeKey, ex => ex.Ignore());
            
            cfg.CreateProjection<Venue, VenueModels.Venue>()
                .ForMember(dto => dto.BannerUri, conf => conf.MapFrom(o => 
                    o.Banner != null 
                        ? new Uri(blobUriTemplate.Replace("{venueId}", o.Id).Replace("{bannerKey}", o.Banner)) 
                        : null));
            cfg.CreateProjection<Opening, VenueModels.Opening>()
                .ForMember(o => o.Day, x => x.MapFrom(o => (int) o.Day))
                .ForMember(o => o.Start, x => x.MapFrom(s =>
                    new VenueModels.Time { Hour = s.StartHour, Minute = s.StartMinute, TimeZone = s.TimeZone }))
                .ForMember(o => o.End, x => x.MapFrom(s => s.EndHour == null? null :
                    new VenueModels.Time { Hour = s.EndHour.Value, Minute = s.EndMinute.Value, TimeZone = s.TimeZone, 
                        NextDay = s.EndHour < s.StartHour || (s.EndHour == s.StartHour && s.EndMinute < s.StartMinute )}));
            cfg.CreateProjection<Location, VenueModels.Location>();
            cfg.CreateProjection<Notice, VenueModels.Notice>();
            cfg.CreateProjection<OpenOverride, VenueModels.OpenOverride>();
        });
        return configuration.CreateMapper();
    }
    
}
