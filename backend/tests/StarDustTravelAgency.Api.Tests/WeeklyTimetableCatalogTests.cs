using StarDustTravelAgency.Api.Domain.Enums;
using StarDustTravelAgency.Api.Persistence.Seeding;

namespace StarDustTravelAgency.Api.Tests;

public sealed class WeeklyTimetableCatalogTests
{
    private const int MinutesPerDay = 24 * 60;
    private const int MinutesPerWeek = 7 * MinutesPerDay;

    [Fact]
    public void Catalog_has_required_routes_legs_and_weekday_coverage()
    {
        var routes = WeeklyTimetableCatalog.CreateRoutes();
        var schedules = WeeklyTimetableCatalog.CreateWeeklySchedules();

        Assert.Equal(30, routes.Count);
        Assert.Equal(60, schedules.Count);
        Assert.Equal(30, schedules.Count(schedule => schedule.Direction == TravelDirection.Outbound));
        Assert.Equal(30, schedules.Count(schedule => schedule.Direction == TravelDirection.Return));
        Assert.Equal(30, routes.Select(route => new { route.LaunchStationId, route.DestinationId }).Distinct().Count());
        Assert.All(routes, route => Assert.InRange(route.DurationMinutes, 1, 16 * 60));
        Assert.Equal(7, schedules.Select(schedule => schedule.DepartureDay).Distinct().Count());

        Assert.All(
            ReferenceDataCatalog.CreateLaunchStations(),
            station => Assert.True(routes.Count(route => route.LaunchStationId == station.Id) >= 2));
        Assert.All(
            ReferenceDataCatalog.CreateDestinations(),
            destination => Assert.True(routes.Count(route => route.DestinationId == destination.Id) >= 2));
    }

    [Fact]
    public void Every_route_has_a_corresponding_return_after_destination_turnaround()
    {
        var plans = WeeklyTimetableCatalog.CreateRoutePlans();
        var schedulesByRoute = WeeklyTimetableCatalog.CreateWeeklySchedules()
            .GroupBy(schedule => schedule.RouteId)
            .ToDictionary(group => group.Key, group => group.ToArray());

        foreach (var plan in plans)
        {
            var pair = schedulesByRoute[plan.RouteId];
            var outbound = Assert.Single(pair, schedule => schedule.Direction == TravelDirection.Outbound);
            var inbound = Assert.Single(pair, schedule => schedule.Direction == TravelDirection.Return);
            var expectedReturnMinute = (
                ToMinuteOfWeek(outbound.DepartureDay, outbound.DepartureTime)
                + plan.DurationMinutes
                + WeeklyTimetableCatalog.TurnaroundMinutes) % MinutesPerWeek;

            Assert.Equal(expectedReturnMinute, ToMinuteOfWeek(inbound.DepartureDay, inbound.DepartureTime));
            Assert.Equal(outbound.DefaultSpaceshipId, inbound.DefaultSpaceshipId);
        }
    }

    [Fact]
    public void Spaceships_have_no_overlaps_including_turnaround_and_week_boundary()
    {
        var routeDurations = WeeklyTimetableCatalog.CreateRoutes()
            .ToDictionary(route => route.Id, route => route.DurationMinutes);
        var schedulesBySpaceship = WeeklyTimetableCatalog.CreateWeeklySchedules()
            .GroupBy(schedule => schedule.DefaultSpaceshipId);

        foreach (var spaceshipSchedules in schedulesBySpaceship)
        {
            var intervals = spaceshipSchedules
                .Select(schedule => new
                {
                    Start = ToMinuteOfWeek(schedule.DepartureDay, schedule.DepartureTime),
                    OccupiedMinutes = routeDurations[schedule.RouteId]
                        + WeeklyTimetableCatalog.TurnaroundMinutes,
                })
                .OrderBy(interval => interval.Start)
                .ToArray();

            for (var index = 0; index < intervals.Length; index++)
            {
                var current = intervals[index];
                var nextStart = index == intervals.Length - 1
                    ? intervals[0].Start + MinutesPerWeek
                    : intervals[index + 1].Start;

                Assert.True(
                    current.Start + current.OccupiedMinutes <= nextStart,
                    $"Spaceship {spaceshipSchedules.Key} has overlapping weekly assignments.");
            }
        }
    }

    private static int ToMinuteOfWeek(DayOfWeek day, TimeOnly time)
    {
        var dayIndex = day == DayOfWeek.Sunday ? 6 : (int)day - 1;
        return (dayIndex * MinutesPerDay) + (time.Hour * 60) + time.Minute;
    }
}
