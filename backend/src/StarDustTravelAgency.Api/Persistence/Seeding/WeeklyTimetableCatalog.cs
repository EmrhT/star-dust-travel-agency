using StarDustTravelAgency.Api.Domain.Entities;
using StarDustTravelAgency.Api.Domain.Enums;
using TravelRoute = StarDustTravelAgency.Api.Domain.Entities.Route;

namespace StarDustTravelAgency.Api.Persistence.Seeding;

public static class WeeklyTimetableCatalog
{
    public const int TurnaroundMinutes = 90;

    private const int MinutesPerDay = 24 * 60;
    private const int MinutesPerWeek = 7 * MinutesPerDay;

    public static IReadOnlyList<WeeklyRoutePlan> CreateRoutePlans() =>
    [
        CreatePlan(1, ReferenceDataIds.LaunchStations.Tokyo, ReferenceDataIds.Destinations.Mars, 260, ReferenceDataIds.Spaceships.CelestialNomad, DayOfWeek.Monday, 6, 0),
        CreatePlan(2, ReferenceDataIds.LaunchStations.Istanbul, ReferenceDataIds.Destinations.Mars, 260, ReferenceDataIds.Spaceships.AuroraVanguard, DayOfWeek.Monday, 8, 0),
        CreatePlan(3, ReferenceDataIds.LaunchStations.Berlin, ReferenceDataIds.Destinations.Mars, 280, ReferenceDataIds.Spaceships.HorizonIX, DayOfWeek.Monday, 10, 0),
        CreatePlan(4, ReferenceDataIds.LaunchStations.Istanbul, ReferenceDataIds.Destinations.Venus, 230, ReferenceDataIds.Spaceships.OdysseyPrime, DayOfWeek.Monday, 12, 0),
        CreatePlan(5, ReferenceDataIds.LaunchStations.Berlin, ReferenceDataIds.Destinations.Venus, 240, ReferenceDataIds.Spaceships.StellarMeridian, DayOfWeek.Monday, 14, 0),
        CreatePlan(6, ReferenceDataIds.LaunchStations.NewYork, ReferenceDataIds.Destinations.Venus, 250, ReferenceDataIds.Spaceships.ArtemisDawn, DayOfWeek.Monday, 16, 0),
        CreatePlan(7, ReferenceDataIds.LaunchStations.Berlin, ReferenceDataIds.Destinations.Titan, 435, ReferenceDataIds.Spaceships.SolarisAscendant, DayOfWeek.Tuesday, 6, 0),
        CreatePlan(8, ReferenceDataIds.LaunchStations.NewYork, ReferenceDataIds.Destinations.Titan, 450, ReferenceDataIds.Spaceships.NebulaVoyager, DayOfWeek.Tuesday, 10, 0),
        CreatePlan(9, ReferenceDataIds.LaunchStations.Tokyo, ReferenceDataIds.Destinations.Titan, 435, ReferenceDataIds.Spaceships.CelestialNomad, DayOfWeek.Wednesday, 0, 0),
        CreatePlan(10, ReferenceDataIds.LaunchStations.NewYork, ReferenceDataIds.Destinations.Europa, 420, ReferenceDataIds.Spaceships.AuroraVanguard, DayOfWeek.Wednesday, 2, 0),
        CreatePlan(11, ReferenceDataIds.LaunchStations.Tokyo, ReferenceDataIds.Destinations.Europa, 400, ReferenceDataIds.Spaceships.HorizonIX, DayOfWeek.Wednesday, 4, 0),
        CreatePlan(12, ReferenceDataIds.LaunchStations.Istanbul, ReferenceDataIds.Destinations.Europa, 410, ReferenceDataIds.Spaceships.OdysseyPrime, DayOfWeek.Wednesday, 6, 0),
        CreatePlan(13, ReferenceDataIds.LaunchStations.Tokyo, ReferenceDataIds.Destinations.ProximaCentauriB, 650, ReferenceDataIds.Spaceships.StellarMeridian, DayOfWeek.Wednesday, 8, 0),
        CreatePlan(14, ReferenceDataIds.LaunchStations.Istanbul, ReferenceDataIds.Destinations.ProximaCentauriB, 645, ReferenceDataIds.Spaceships.ArtemisDawn, DayOfWeek.Wednesday, 10, 0),
        CreatePlan(15, ReferenceDataIds.LaunchStations.Berlin, ReferenceDataIds.Destinations.ProximaCentauriB, 670, ReferenceDataIds.Spaceships.SolarisAscendant, DayOfWeek.Thursday, 14, 0),
        CreatePlan(16, ReferenceDataIds.LaunchStations.Istanbul, ReferenceDataIds.Destinations.Trappist1E, 755, ReferenceDataIds.Spaceships.NebulaVoyager, DayOfWeek.Thursday, 18, 0),
        CreatePlan(17, ReferenceDataIds.LaunchStations.Berlin, ReferenceDataIds.Destinations.Trappist1E, 770, ReferenceDataIds.Spaceships.CelestialNomad, DayOfWeek.Thursday, 18, 0),
        CreatePlan(18, ReferenceDataIds.LaunchStations.NewYork, ReferenceDataIds.Destinations.Trappist1E, 790, ReferenceDataIds.Spaceships.AuroraVanguard, DayOfWeek.Thursday, 20, 0),
        CreatePlan(19, ReferenceDataIds.LaunchStations.Berlin, ReferenceDataIds.Destinations.Kepler186F, 930, ReferenceDataIds.Spaceships.HorizonIX, DayOfWeek.Thursday, 22, 0),
        CreatePlan(20, ReferenceDataIds.LaunchStations.NewYork, ReferenceDataIds.Destinations.Kepler186F, 940, ReferenceDataIds.Spaceships.OdysseyPrime, DayOfWeek.Friday, 0, 0),
        CreatePlan(21, ReferenceDataIds.LaunchStations.Tokyo, ReferenceDataIds.Destinations.Kepler186F, 940, ReferenceDataIds.Spaceships.StellarMeridian, DayOfWeek.Friday, 2, 0),
        CreatePlan(22, ReferenceDataIds.LaunchStations.NewYork, ReferenceDataIds.Destinations.Ganymede, 380, ReferenceDataIds.Spaceships.ArtemisDawn, DayOfWeek.Friday, 4, 0),
        CreatePlan(23, ReferenceDataIds.LaunchStations.Tokyo, ReferenceDataIds.Destinations.Ganymede, 365, ReferenceDataIds.Spaceships.SolarisAscendant, DayOfWeek.Saturday, 22, 0),
        CreatePlan(24, ReferenceDataIds.LaunchStations.Istanbul, ReferenceDataIds.Destinations.Ganymede, 370, ReferenceDataIds.Spaceships.NebulaVoyager, DayOfWeek.Sunday, 2, 0),
        CreatePlan(25, ReferenceDataIds.LaunchStations.Tokyo, ReferenceDataIds.Destinations.Enceladus, 470, ReferenceDataIds.Spaceships.CelestialNomad, DayOfWeek.Saturday, 12, 0),
        CreatePlan(26, ReferenceDataIds.LaunchStations.Istanbul, ReferenceDataIds.Destinations.Enceladus, 480, ReferenceDataIds.Spaceships.AuroraVanguard, DayOfWeek.Saturday, 14, 0),
        CreatePlan(27, ReferenceDataIds.LaunchStations.Berlin, ReferenceDataIds.Destinations.Enceladus, 490, ReferenceDataIds.Spaceships.HorizonIX, DayOfWeek.Saturday, 16, 0),
        CreatePlan(28, ReferenceDataIds.LaunchStations.Istanbul, ReferenceDataIds.Destinations.Kepler452B, 840, ReferenceDataIds.Spaceships.OdysseyPrime, DayOfWeek.Saturday, 18, 0),
        CreatePlan(29, ReferenceDataIds.LaunchStations.Berlin, ReferenceDataIds.Destinations.Kepler452B, 860, ReferenceDataIds.Spaceships.StellarMeridian, DayOfWeek.Saturday, 20, 0),
        CreatePlan(30, ReferenceDataIds.LaunchStations.NewYork, ReferenceDataIds.Destinations.Kepler452B, 880, ReferenceDataIds.Spaceships.ArtemisDawn, DayOfWeek.Saturday, 22, 0),
    ];

    public static IReadOnlyList<TravelRoute> CreateRoutes() =>
        CreateRoutePlans()
            .Select(plan => new TravelRoute
            {
                Id = plan.RouteId,
                LaunchStationId = plan.LaunchStationId,
                DestinationId = plan.DestinationId,
                DurationMinutes = plan.DurationMinutes,
            })
            .ToArray();

    public static IReadOnlyList<WeeklySchedule> CreateWeeklySchedules() =>
        CreateRoutePlans()
            .SelectMany(CreateSchedulePair)
            .ToArray();

    private static IEnumerable<WeeklySchedule> CreateSchedulePair(WeeklyRoutePlan plan)
    {
        var returnDeparture = FromMinuteOfWeek(
            ToMinuteOfWeek(plan.OutboundDay, plan.OutboundTime)
            + plan.DurationMinutes
            + TurnaroundMinutes);

        yield return new WeeklySchedule
        {
            Id = CreateDeterministicId(5, plan.Sequence),
            RouteId = plan.RouteId,
            Direction = TravelDirection.Outbound,
            DepartureDay = plan.OutboundDay,
            DepartureTime = plan.OutboundTime,
            DefaultSpaceshipId = plan.SpaceshipId,
        };

        yield return new WeeklySchedule
        {
            Id = CreateDeterministicId(6, plan.Sequence),
            RouteId = plan.RouteId,
            Direction = TravelDirection.Return,
            DepartureDay = returnDeparture.Day,
            DepartureTime = returnDeparture.Time,
            DefaultSpaceshipId = plan.SpaceshipId,
        };
    }

    private static WeeklyRoutePlan CreatePlan(
        int sequence,
        Guid launchStationId,
        Guid destinationId,
        int durationMinutes,
        Guid spaceshipId,
        DayOfWeek outboundDay,
        int outboundHour,
        int outboundMinute) =>
        new(
            sequence,
            CreateDeterministicId(4, sequence),
            launchStationId,
            destinationId,
            durationMinutes,
            spaceshipId,
            outboundDay,
            new TimeOnly(outboundHour, outboundMinute));

    private static Guid CreateDeterministicId(int category, int sequence) =>
        Guid.Parse($"{category}0000000-0000-0000-0000-{sequence:D12}");

    private static int ToMinuteOfWeek(DayOfWeek day, TimeOnly time)
    {
        var dayIndex = day == DayOfWeek.Sunday ? 6 : (int)day - 1;
        return (dayIndex * MinutesPerDay) + (time.Hour * 60) + time.Minute;
    }

    private static (DayOfWeek Day, TimeOnly Time) FromMinuteOfWeek(int minute)
    {
        var normalizedMinute = ((minute % MinutesPerWeek) + MinutesPerWeek) % MinutesPerWeek;
        var dayIndex = normalizedMinute / MinutesPerDay;
        var minuteOfDay = normalizedMinute % MinutesPerDay;
        var day = dayIndex == 6 ? DayOfWeek.Sunday : (DayOfWeek)(dayIndex + 1);

        return (day, new TimeOnly(minuteOfDay / 60, minuteOfDay % 60));
    }
}
