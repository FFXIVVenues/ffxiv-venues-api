using AutoMapper;

namespace FFXIVVenues.DomainData.Mapping;

public interface IMapFactory
{

    public IMapper GetModelMapper();

    public IMapper GetModelProjector();
    
}