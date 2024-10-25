using FFXIVVenues.Api.PersistenceModels.Entities;
using FFXIVVenues.Api.PersistenceModels.Entities.Venues;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace FFXIVVenues.Api.PersistenceModels.Context;

public class FFXIVVenuesDbContext : DbContext
{
    private readonly string _connectionString;
    public DbSet<Venue> Venues { get; set; }
    public DbSet<VenueView> VenueViews { get; set; }

    public FFXIVVenuesDbContext(IConfiguration configuration) 
    {
        this._connectionString = configuration.GetConnectionString("FFXIVVenues");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Venues");
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseNpgsql(this._connectionString,
                o => o.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "Venues"));
        optionsBuilder.UseLazyLoadingProxies();
    }
}
