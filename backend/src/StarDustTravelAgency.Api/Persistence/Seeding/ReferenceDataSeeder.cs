using Microsoft.EntityFrameworkCore;
using StarDustTravelAgency.Api.Domain.Entities;

// Inserts missing reference catalog records through an EF Core DbContext.
// Program registers both paths with EF Core's migration-time seed hooks.
namespace StarDustTravelAgency.Api.Persistence.Seeding;

/// <summary>
/// Inserts missing entities supplied by ReferenceDataCatalog into DbContext.
/// Program connects Seed and SeedAsync to EF Core seeding hooks.
/// </summary>
public static class ReferenceDataSeeder
{
    // Adds missing catalog records and commits them for synchronous EF calls.
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

    // Adds missing catalog records and commits them for asynchronous EF calls.
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

    // Compares stored station IDs with ReferenceDataCatalog before tracking.
    private static void AddMissingLaunchStations(DbContext context)
    {
        var existingIds = context.Set<LaunchStation>()
            .Select(station => station.Id)
            .ToHashSet();

        context.AddRange(ReferenceDataCatalog.CreateLaunchStations()
            .Where(station => !existingIds.Contains(station.Id)));
    }

    // Compares stored destination IDs with the catalog before tracking.
    private static void AddMissingDestinations(DbContext context)
    {
        var existingIds = context.Set<Destination>()
            .Select(destination => destination.Id)
            .ToHashSet();

        context.AddRange(ReferenceDataCatalog.CreateDestinations()
            .Where(destination => !existingIds.Contains(destination.Id)));
    }

    // Compares stored spaceship IDs with ReferenceDataCatalog before tracking.
    private static void AddMissingSpaceships(DbContext context)
    {
        var existingIds = context.Set<Spaceship>()
            .Select(spaceship => spaceship.Id)
            .ToHashSet();

        context.AddRange(ReferenceDataCatalog.CreateSpaceships()
            .Where(spaceship => !existingIds.Contains(spaceship.Id)));
    }

    // Asynchronously tracks stations absent from the connected DbContext.
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

    // Asynchronously tracks destinations absent from the connected DbContext.
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

    // Asynchronously tracks spaceships absent from the connected DbContext.
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
