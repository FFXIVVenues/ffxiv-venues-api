using System.Collections.Generic;
using AutoMapper;
using FFXIVVenues.Api.PersistenceModels.Entities.Venues;

namespace FFXIVVenues.Api.PersistenceModels.Mapping;

public static class AutoMapping
{

    public static IMapper GetModelMapper(ISplashUriMapper splashUriMapper)
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(Venue));
            cfg.CreateMap<Venue, FFXIVVenues.VenueModels.Venue>()
                .ForMember(d => d.BannerUri, o => o.MapFrom(splashUriMapper));
            cfg.CreateMap<FFXIVVenues.VenueModels.Venue, Venue>()
                .ForMember(d => d.Openings, o => o.MapFrom<InPlaceListMemberResolver<VenueModels.Opening, Opening>, List<VenueModels.Opening>>(s => s.Openings))
                .ForMember(d => d.Notices,  o => o.MapFrom<InPlaceListMemberResolver<VenueModels.Notice, Notice>, List<VenueModels.Notice>>(s => s.Notices))
                .ForMember(d => d.OpenOverrides,  o => o.MapFrom<InPlaceListMemberResolver<VenueModels.OpenOverride, OpenOverride>, List<VenueModels.OpenOverride>>(s => s.OpenOverrides));
        });
        return configuration.CreateMapper();
    }
    
}

