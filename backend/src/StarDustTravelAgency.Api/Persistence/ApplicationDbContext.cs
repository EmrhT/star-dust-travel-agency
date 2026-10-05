using Microsoft.EntityFrameworkCore;
using StarDustTravelAgency.Api.Domain.Entities;
using TravelRoute = StarDustTravelAgency.Api.Domain.Entities.Route;

namespace StarDustTravelAgency.Api.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<Destination> Destinations => Set<Destination>();

    public DbSet<Journey> Journeys => Set<Journey>();

    public DbSet<LaunchStation> LaunchStations => Set<LaunchStation>();

    public DbSet<TravelRoute> Routes => Set<TravelRoute>();

    public DbSet<Spaceship> Spaceships => Set<Spaceship>();

    public DbSet<WeeklySchedule> WeeklySchedules => Set<WeeklySchedule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
