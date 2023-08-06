using Microsoft.Extensions.Configuration;

namespace FFXIVVenues.Api.PersistenceModels.Context;

public interface IFFXIVVenuesDbContextFactory
{
    public FFXIVVenuesDbContext Create();
} 

public class FFXIVVenuesDbContextFactory : IFFXIVVenuesDbContextFactory
{
    private readonly IConfiguration _config;

    public FFXIVVenuesDbContextFactory(IConfiguration config)
    {
        _config = config;
    }


    public FFXIVVenuesDbContext Create()
    {
        return new FFXIVVenuesDbContext(this._config);
    }
}