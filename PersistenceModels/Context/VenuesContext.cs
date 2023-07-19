using FFXIVVenues.Api.PersistenceModels.Entities;
using FFXIVVenues.Api.PersistenceModels.Entities.Venues;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace FFXIVVenues.Api.PersistenceModels.Context;

public class VenuesContext : DbContext
{
    public DbSet<Venue> Venues { get; set; }
    public DbSet<VenueView> VenueViews { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Venues");
        base.OnModelCreating(modelBuilder);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .UseNpgsql("",
                o => o.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "Venues"))
            .UseLazyLoadingProxies();
}