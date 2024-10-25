using AutoMapper;

namespace FFXIVVenues.Api.PersistenceModels.Mapping;

public interface IMapFactory
{

    public IMapper GetModelMapper();

    public IMapper GetModelProjector();
    
}