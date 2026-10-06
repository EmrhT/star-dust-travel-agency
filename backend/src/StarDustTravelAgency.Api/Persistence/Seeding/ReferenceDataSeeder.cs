using Microsoft.EntityFrameworkCore;
using StarDustTravelAgency.Api.Domain.Entities;
using TravelRoute = StarDustTravelAgency.Api.Domain.Entities.Route;

// Inserts missing reference and timetable records through an EF Core DbContext.
// Program registers both paths with EF Core's migration-time seed hooks.
namespace StarDustTravelAgency.Api.Persistence.Seeding;

/// <summary>
/// Inserts entities from reference and timetable catalogs into DbContext.
/// Program connects Seed and SeedAsync to EF Core seeding hooks.
/// </summary>
public static class ReferenceDataSeeder
{
    // Adds catalogs in foreign-key order and commits synchronous EF calls.
    public static void Seed(DbContext context)
    {
        AddMissingLaunchStations(context);
        AddMissingDestinations(context);
        AddMissingSpaceships(context);
        AddMissingRoutes(context);
        AddMissingWeeklySchedules(context);

        if (context.ChangeTracker.HasChanges())
        {
            context.SaveChanges();
        }
    }

    // Adds catalogs in foreign-key order and commits asynchronous EF calls.
    public static async Task SeedAsync(
        DbContext context,
        CancellationToken cancellationToken = default)
    {
        await AddMissingLaunchStationsAsync(context, cancellationToken);
        await AddMissingDestinationsAsync(context, cancellationToken);
        await AddMissingSpaceshipsAsync(context, cancellationToken);
        await AddMissingRoutesAsync(context, cancellationToken);
        await AddMissingWeeklySchedulesAsync(context, cancellationToken);

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

    // Compares stored route IDs with WeeklyTimetableCatalog before tracking.
    private static void AddMissingRoutes(DbContext context)
    {
        var existingIds = context.Set<TravelRoute>()
            .Select(route => route.Id)
            .ToHashSet();

        context.AddRange(WeeklyTimetableCatalog.CreateRoutes()
            .Where(route => !existingIds.Contains(route.Id)));
    }

    // Compares stored schedule IDs with the timetable before tracking.
    private static void AddMissingWeeklySchedules(DbContext context)
    {
        var existingIds = context.Set<WeeklySchedule>()
            .Select(schedule => schedule.Id)
            .ToHashSet();

        context.AddRange(WeeklyTimetableCatalog.CreateWeeklySchedules()
            .Where(schedule => !existingIds.Contains(schedule.Id)));
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

    // Asynchronously tracks routes absent from the connected DbContext.
    private static async Task AddMissingRoutesAsync(
        DbContext context,
        CancellationToken cancellationToken)
    {
        var existingIds = await context.Set<TravelRoute>()
            .Select(route => route.Id)
            .ToHashSetAsync(cancellationToken);

        await context.AddRangeAsync(
            WeeklyTimetableCatalog.CreateRoutes()
                .Where(route => !existingIds.Contains(route.Id)),
            cancellationToken);
    }

    // Asynchronously tracks schedules absent from the connected DbContext.
    private static async Task AddMissingWeeklySchedulesAsync(
        DbContext context,
        CancellationToken cancellationToken)
    {
        var existingIds = await context.Set<WeeklySchedule>()
            .Select(schedule => schedule.Id)
            .ToHashSetAsync(cancellationToken);

        await context.AddRangeAsync(
            WeeklyTimetableCatalog.CreateWeeklySchedules()
                .Where(schedule => !existingIds.Contains(schedule.Id)),
            cancellationToken);
    }
}
