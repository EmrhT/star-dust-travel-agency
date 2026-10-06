using Microsoft.EntityFrameworkCore;
using StarDustTravelAgency.Api.Domain.Entities;
using TravelRoute = StarDustTravelAgency.Api.Domain.Entities.Route;

// Defines the EF Core unit of work that connects domain entities to PostgreSQL.
// Program registers it, while configuration classes supply relational mappings.
namespace StarDustTravelAgency.Api.Persistence;

/// <summary>
/// Exposes domain DbSets and loads all entity configuration classes.
/// Program registers it and ReferenceDataSeeder populates it.
/// </summary>
public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<Destination> Destinations => Set<Destination>();

    public DbSet<Journey> Journeys => Set<Journey>();

    public DbSet<LaunchStation> LaunchStations => Set<LaunchStation>();

    public DbSet<TravelRoute> Routes => Set<TravelRoute>();

    public DbSet<Spaceship> Spaceships => Set<Spaceship>();

    public DbSet<WeeklySchedule> WeeklySchedules => Set<WeeklySchedule>();

    // Loads every entity mapping class from the API assembly into EF Core.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }
}
