using Microsoft.EntityFrameworkCore;
using StarDustTravelAgency.Api.Domain.Entities;
using StarDustTravelAgency.Api.Domain.Enums;
using StarDustTravelAgency.Api.Domain.Time;
using StarDustTravelAgency.Api.Persistence;

// Creates dated journeys from schedules using shared BusinessTime conversion.
// JourneysController supplies input; ApplicationDbContext persists output.
namespace StarDustTravelAgency.Api.Services.Journeys;

/// <summary>
/// Converts active WeeklySchedule rows into idempotent Journey rows.
/// BusinessTime supplies UTC values; JourneyConfiguration ensures uniqueness.
/// </summary>
public sealed class JourneyGenerationService(
    ApplicationDbContext context)
{
    /// <summary>
    /// Generates BusinessTime-aligned journeys for JourneysController.
    /// ApplicationDbContext persists only missing schedule occurrences.
    /// </summary>
    public async Task<JourneyGenerationResult> GenerateWeekAsync(
        DateOnly weekStarting,
        CancellationToken cancellationToken = default)
    {
        var schedules = await context.WeeklySchedules
            .AsNoTracking()
            .Where(schedule => schedule.Active && schedule.Route.Active)
            .Select(schedule => new
            {
                schedule.Id,
                schedule.RouteId,
                schedule.Direction,
                schedule.DepartureDay,
                schedule.DepartureTime,
                schedule.DefaultSpaceshipId,
                schedule.Route.DurationMinutes,
            })
            .ToArrayAsync(cancellationToken);

        var weekStartUtc = BusinessTime.StartOfDayUtc(weekStarting);
        var weekEndUtc = weekStartUtc.AddDays(7);
        var storedOccurrences = await context.Journeys
            .AsNoTracking()
            .Where(journey =>
                journey.WeeklyScheduleId.HasValue
                && journey.DepartureAtUtc >= weekStartUtc
                && journey.DepartureAtUtc < weekEndUtc)
            .Select(journey => new
            {
                WeeklyScheduleId = journey.WeeklyScheduleId!.Value,
                journey.DepartureAtUtc,
            })
            .ToArrayAsync(cancellationToken);
        var existingOccurrences = storedOccurrences
            .Select(journey =>
                (journey.WeeklyScheduleId, journey.DepartureAtUtc))
            .ToHashSet();

        var newJourneys = new List<Journey>();

        foreach (var schedule in schedules)
        {
            var departureDate = weekStarting.AddDays(
                ToMondayBasedOffset(schedule.DepartureDay));
            var departureAtUtc = BusinessTime.ToUtc(
                departureDate,
                schedule.DepartureTime);
            var occurrence = (schedule.Id, departureAtUtc);

            if (!existingOccurrences.Add(occurrence))
            {
                continue;
            }

            newJourneys.Add(new Journey
            {
                Id = Guid.NewGuid(),
                RouteId = schedule.RouteId,
                WeeklyScheduleId = schedule.Id,
                Direction = schedule.Direction,
                DepartureAtUtc = departureAtUtc,
                ArrivalAtUtc = departureAtUtc.AddMinutes(
                    schedule.DurationMinutes),
                SpaceshipId = schedule.DefaultSpaceshipId,
                Status = JourneyStatus.Scheduled,
            });
        }

        context.Journeys.AddRange(newJourneys);
        await context.SaveChangesAsync(cancellationToken);

        return new JourneyGenerationResult(
            newJourneys.Count,
            schedules.Length - newJourneys.Count);
    }

    /// <summary>
    /// Maps WeeklySchedule.DayOfWeek to its offset from the requested Monday.
    /// </summary>
    private static int ToMondayBasedOffset(DayOfWeek day) =>
        day == DayOfWeek.Sunday ? 6 : (int)day - 1;
}

/// <summary>
/// Carries created and existing counts from the service to the controller.
/// JourneysController converts it into GenerateJourneyWeekResponse JSON.
/// </summary>
public sealed record JourneyGenerationResult(
    int Created,
    int AlreadyExisted);
