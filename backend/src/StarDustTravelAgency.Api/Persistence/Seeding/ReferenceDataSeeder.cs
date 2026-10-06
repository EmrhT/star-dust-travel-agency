using Microsoft.EntityFrameworkCore;
using StarDustTravelAgency.Api.Domain.Entities;

namespace StarDustTravelAgency.Api.Persistence.Seeding;

public static class ReferenceDataSeeder
{
    public static void Seed(DbContext context)
    {
        AddMissingLaunchStations(context);
        AddMissingDestinations(context);
        AddMissingSpaceships(context);

        if (context.ChangeTracker.HasChanges())
        {
            context.SaveChanges();
        }
    }

    public static async Task SeedAsync(
        DbContext context,
        CancellationToken cancellationToken = default)
    {
        await AddMissingLaunchStationsAsync(context, cancellationToken);
        await AddMissingDestinationsAsync(context, cancellationToken);
        await AddMissingSpaceshipsAsync(context, cancellationToken);

        if (context.ChangeTracker.HasChanges())
        {
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    private static void AddMissingLaunchStations(DbContext context)
    {
        var existingIds = context.Set<LaunchStation>()
            .Select(station => station.Id)
            .ToHashSet();

        context.AddRange(ReferenceDataCatalog.CreateLaunchStations()
            .Where(station => !existingIds.Contains(station.Id)));
    }

    private static void AddMissingDestinations(DbContext context)
    {
        var existingIds = context.Set<Destination>()
            .Select(destination => destination.Id)
            .ToHashSet();

        context.AddRange(ReferenceDataCatalog.CreateDestinations()
            .Where(destination => !existingIds.Contains(destination.Id)));
    }

    private static void AddMissingSpaceships(DbContext context)
    {
        var existingIds = context.Set<Spaceship>()
            .Select(spaceship => spaceship.Id)
            .ToHashSet();

        context.AddRange(ReferenceDataCatalog.CreateSpaceships()
            .Where(spaceship => !existingIds.Contains(spaceship.Id)));
    }

    private static async Task AddMissingLaunchStationsAsync(
        DbContext context,
        CancellationToken cancellationToken)
    {
        var existingIds = await context.Set<LaunchStation>()
            .Select(station => station.Id)
            .ToHashSetAsync(cancellationToken);

        await context.AddRangeAsync(
            ReferenceDataCatalog.CreateLaunchStations()
                .Where(station => !existingIds.Contains(station.Id)),
            cancellationToken);
    }

    private static async Task AddMissingDestinationsAsync(
        DbContext context,
        CancellationToken cancellationToken)
    {
        var existingIds = await context.Set<Destination>()
            .Select(destination => destination.Id)
            .ToHashSetAsync(cancellationToken);

        await context.AddRangeAsync(
            ReferenceDataCatalog.CreateDestinations()
                .Where(destination => !existingIds.Contains(destination.Id)),
            cancellationToken);
    }

    private static async Task AddMissingSpaceshipsAsync(
        DbContext context,
        CancellationToken cancellationToken)
    {
        var existingIds = await context.Set<Spaceship>()
            .Select(spaceship => spaceship.Id)
            .ToHashSetAsync(cancellationToken);

        await context.AddRangeAsync(
            ReferenceDataCatalog.CreateSpaceships()
                .Where(spaceship => !existingIds.Contains(spaceship.Id)),
            cancellationToken);
    }
}
