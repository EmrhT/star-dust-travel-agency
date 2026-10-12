using Microsoft.EntityFrameworkCore;
using StarDustTravelAgency.Api.Domain.Enums;
using StarDustTravelAgency.Api.Domain.Time;
using StarDustTravelAgency.Api.Persistence;

// Reads one BusinessTime-bounded week through EF Core projections.
// JourneysController returns the resulting read models as JSON to API clients.
namespace StarDustTravelAgency.Api.Services.Journeys;

/// <summary>
/// Queries ApplicationDbContext for display-ready weekly journey data.
/// BusinessTime supplies bounds; JourneysController serializes the items.
/// </summary>
public sealed class JourneyQueryService(
    ApplicationDbContext context)
{
    /// <summary>
    /// Returns one BusinessTime-bounded week ordered by UTC departure.
    /// JourneysController awaits the projected JourneyListItem collection.
    /// </summary>
    public async Task<IReadOnlyList<JourneyListItem>> GetWeekAsync(
        DateOnly weekStarting,
        CancellationToken cancellationToken = default)
    {
        var weekStartUtc = BusinessTime.StartOfDayUtc(weekStarting);
        var weekEndUtc = weekStartUtc.AddDays(7);

        return await context.Journeys
            .AsNoTracking()
            .Where(journey =>
                journey.DepartureAtUtc >= weekStartUtc
                && journey.DepartureAtUtc < weekEndUtc)
            .OrderBy(journey => journey.DepartureAtUtc)
            .Select(journey => new JourneyListItem(
                journey.Id,
                journey.Route.LaunchStation.Name,
                journey.Route.Destination.Name,
                journey.Direction,
                journey.DepartureAtUtc,
                journey.ArrivalAtUtc,
                journey.Spaceship.Name,
                journey.Status))
            .ToArrayAsync(cancellationToken);
    }
}

/// <summary>
/// Projects Journey and related entity fields needed by the future React view.
/// JourneyQueryService creates these items without exposing EF entities.
/// </summary>
public sealed record JourneyListItem(
    Guid Id,
    string LaunchStation,
    string Destination,
    TravelDirection Direction,
    DateTimeOffset DepartureAtUtc,
    DateTimeOffset ArrivalAtUtc,
    string Spaceship,
    JourneyStatus Status);
