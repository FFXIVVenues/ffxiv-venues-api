using AutoMapper;
using FFXIVVenues.Api.PersistenceModels.Entities.Venues;

namespace FFXIVVenues.Api.PersistenceModels.Mapping;

public static class AutoMapping
{

    public static IMapper GetModelMapper(ISplashUriMapper splashUriMapper)
    {
        var configuration = new MapperConfiguration(cfg =>
        {
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
            cfg.CreateMap<OpenOverride, VenueModels.OpenOverride>().ReverseMap();
            cfg.CreateMap<Location, VenueModels.Location>().ReverseMap();
            cfg.CreateMap<Notice, VenueModels.Notice>().ReverseMap();
            cfg.CreateMap<Day, VenueModels.Day>().ReverseMap();
            cfg.CreateMap<Time, VenueModels.Time>().ReverseMap();
            cfg.CreateMap<Venue, FFXIVVenues.VenueModels.Venue>()
                .ForMember(d => d.BannerUri, o => o.MapFrom(splashUriMapper));
            cfg.CreateMap<FFXIVVenues.VenueModels.Venue, Venue>()
                .ForMember(d => d.Added, ex => ex.Ignore())
                .ForMember(d => d.LastModified, ex => ex.Ignore())
                .ForMember(d => d.Approved, ex => ex.Ignore())
                .ForMember(d => d.ScopeKey, ex => ex.Ignore());
            // .ForMember(d => d.Openings,  o => o.MapFrom<InPlaceListValueResolver<VenueModels.Opening, Opening>, List<VenueModels.Opening>>(s => s.Openings))
            // .ForMember(d => d.Notices,
            //     o => o.MapFrom<InPlaceListValueResolver<VenueModels.Notice, Notice>, List<VenueModels.Notice>>(s =>
            //         s.Notices));
            // .ForMember(d => d.OpenOverrides,  o => o.MapFrom<InPlaceListValueResolver<VenueModels.OpenOverride, OpenOverride>, List<VenueModels.OpenOverride>>(s => s.OpenOverrides));
        });
        return configuration.CreateMapper();
    }
    
}

public class DtoToDomainVenueConverter : ITypeConverter<VenueModels.Venue, Venue>
{
    public Venue Convert(VenueModels.Venue source, Venue destination, ResolutionContext context)
    {
        var newVenue = context.Mapper.Map<Venue>(source);
        return newVenue;
    }
}

