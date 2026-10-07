using Microsoft.EntityFrameworkCore;
using StarDustTravelAgency.Api.Domain.Enums;
using StarDustTravelAgency.Api.Persistence;

// Reads dated journeys for one requested week through EF Core projections.
// JourneysController returns the resulting read models as JSON to API clients.
namespace StarDustTravelAgency.Api.Services.Journeys;

/// <summary>
/// Queries ApplicationDbContext for display-ready weekly journey data.
/// JourneysController supplies the week and serializes the returned items.
/// </summary>
public sealed class JourneyQueryService(
    ApplicationDbContext context)
{
    private static readonly TimeSpan UtcPlusThree = TimeSpan.FromHours(3);

    /// <summary>
    /// Returns one UTC+3 schedule week ordered by its UTC departure time.
    /// </summary>
    public async Task<IReadOnlyList<JourneyListItem>> GetWeekAsync(
        DateOnly weekStarting,
        CancellationToken cancellationToken = default)
    {
        var weekStartUtc = ToUtc(weekStarting);
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

    /// <summary>
    /// Converts the requested UTC+3 Monday boundary into a UTC timestamp.
    /// </summary>
    private static DateTimeOffset ToUtc(DateOnly date)
    {
        var localDateTime = date.ToDateTime(TimeOnly.MinValue);
        return new DateTimeOffset(localDateTime, UtcPlusThree)
            .ToUniversalTime();
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
