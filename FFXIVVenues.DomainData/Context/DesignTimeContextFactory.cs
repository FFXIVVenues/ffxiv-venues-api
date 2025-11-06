using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FFXIVVenues.DomainData.Context;

internal class DesignTimeContextFactory() : IDesignTimeDbContextFactory<DomainDataContext>
{
    public DomainDataContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<DesignTimeContextFactory>()
            .Build(); 
        var connectionString = config.GetConnectionString("FFXIVVenues");
        var options = new DbContextOptionsBuilder<DomainDataContext>().UseNpgsql(connectionString).Options;
        return new DomainDataContext(options);
    }
}