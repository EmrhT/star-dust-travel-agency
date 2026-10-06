using StarDustTravelAgency.Api.Domain.Entities;
using StarDustTravelAgency.Api.Domain.Enums;
using TravelRoute = StarDustTravelAgency.Api.Domain.Entities.Route;

// Builds a deterministic route network and conflict-free weekly timetable.
// It joins ReferenceDataIds to Route and WeeklySchedule domain entities.
namespace StarDustTravelAgency.Api.Persistence.Seeding;

using DestinationIds = ReferenceDataIds.Destinations;
using SpaceshipIds = ReferenceDataIds.Spaceships;
using StationIds = ReferenceDataIds.LaunchStations;

/// <summary>
/// Builds Route and WeeklySchedule entities from ReferenceDataIds.
/// WeeklyTimetableCatalogTests validate its timing and fleet rules.
/// </summary>
public static class WeeklyTimetableCatalog
{
    public const int TurnaroundMinutes = 90;

    private const int MinutesPerDay = 24 * 60;
    private const int MinutesPerWeek = 7 * MinutesPerDay;

    // Creates the common plans used by route and schedule factory methods.
    public static IReadOnlyList<WeeklyRoutePlan> CreateRoutePlans() =>
    [
        CreatePlan(
            1, StationIds.Tokyo, DestinationIds.Mars, 260,
            SpaceshipIds.CelestialNomad, DayOfWeek.Monday, 6, 0),
        CreatePlan(
            2, StationIds.Istanbul, DestinationIds.Mars, 260,
            SpaceshipIds.AuroraVanguard, DayOfWeek.Monday, 8, 0),
        CreatePlan(
            3, StationIds.Berlin, DestinationIds.Mars, 280,
            SpaceshipIds.HorizonIX, DayOfWeek.Monday, 10, 0),
        CreatePlan(
            4, StationIds.Istanbul, DestinationIds.Venus, 230,
            SpaceshipIds.OdysseyPrime, DayOfWeek.Monday, 12, 0),
        CreatePlan(
            5, StationIds.Berlin, DestinationIds.Venus, 240,
            SpaceshipIds.StellarMeridian, DayOfWeek.Monday, 14, 0),
        CreatePlan(
            6, StationIds.NewYork, DestinationIds.Venus, 250,
            SpaceshipIds.ArtemisDawn, DayOfWeek.Monday, 16, 0),
        CreatePlan(
            7, StationIds.Berlin, DestinationIds.Titan, 435,
            SpaceshipIds.SolarisAscendant, DayOfWeek.Tuesday, 6, 0),
        CreatePlan(
            8, StationIds.NewYork, DestinationIds.Titan, 450,
            SpaceshipIds.NebulaVoyager, DayOfWeek.Tuesday, 10, 0),
        CreatePlan(
            9, StationIds.Tokyo, DestinationIds.Titan, 435,
            SpaceshipIds.CelestialNomad, DayOfWeek.Wednesday, 0, 0),
        CreatePlan(
            10, StationIds.NewYork, DestinationIds.Europa, 420,
            SpaceshipIds.AuroraVanguard, DayOfWeek.Wednesday, 2, 0),
        CreatePlan(
            11, StationIds.Tokyo, DestinationIds.Europa, 400,
            SpaceshipIds.HorizonIX, DayOfWeek.Wednesday, 4, 0),
        CreatePlan(
            12, StationIds.Istanbul, DestinationIds.Europa, 410,
            SpaceshipIds.OdysseyPrime, DayOfWeek.Wednesday, 6, 0),
        CreatePlan(
            13, StationIds.Tokyo, DestinationIds.ProximaCentauriB, 650,
            SpaceshipIds.StellarMeridian, DayOfWeek.Wednesday, 8, 0),
        CreatePlan(
            14, StationIds.Istanbul, DestinationIds.ProximaCentauriB, 645,
            SpaceshipIds.ArtemisDawn, DayOfWeek.Wednesday, 10, 0),
        CreatePlan(
            15, StationIds.Berlin, DestinationIds.ProximaCentauriB, 670,
            SpaceshipIds.SolarisAscendant, DayOfWeek.Thursday, 14, 0),
        CreatePlan(
            16, StationIds.Istanbul, DestinationIds.Trappist1E, 755,
            SpaceshipIds.NebulaVoyager, DayOfWeek.Thursday, 18, 0),
        CreatePlan(
            17, StationIds.Berlin, DestinationIds.Trappist1E, 770,
            SpaceshipIds.CelestialNomad, DayOfWeek.Thursday, 18, 0),
        CreatePlan(
            18, StationIds.NewYork, DestinationIds.Trappist1E, 790,
            SpaceshipIds.AuroraVanguard, DayOfWeek.Thursday, 20, 0),
        CreatePlan(
            19, StationIds.Berlin, DestinationIds.Kepler186F, 930,
            SpaceshipIds.HorizonIX, DayOfWeek.Thursday, 22, 0),
        CreatePlan(
            20, StationIds.NewYork, DestinationIds.Kepler186F, 940,
            SpaceshipIds.OdysseyPrime, DayOfWeek.Friday, 0, 0),
        CreatePlan(
            21, StationIds.Tokyo, DestinationIds.Kepler186F, 940,
            SpaceshipIds.StellarMeridian, DayOfWeek.Friday, 2, 0),
        CreatePlan(
            22, StationIds.NewYork, DestinationIds.Ganymede, 380,
            SpaceshipIds.ArtemisDawn, DayOfWeek.Friday, 4, 0),
        CreatePlan(
            23, StationIds.Tokyo, DestinationIds.Ganymede, 365,
            SpaceshipIds.SolarisAscendant, DayOfWeek.Saturday, 22, 0),
        CreatePlan(
            24, StationIds.Istanbul, DestinationIds.Ganymede, 370,
            SpaceshipIds.NebulaVoyager, DayOfWeek.Sunday, 2, 0),
        CreatePlan(
            25, StationIds.Tokyo, DestinationIds.Enceladus, 470,
            SpaceshipIds.CelestialNomad, DayOfWeek.Saturday, 12, 0),
        CreatePlan(
            26, StationIds.Istanbul, DestinationIds.Enceladus, 480,
            SpaceshipIds.AuroraVanguard, DayOfWeek.Saturday, 14, 0),
        CreatePlan(
            27, StationIds.Berlin, DestinationIds.Enceladus, 490,
            SpaceshipIds.HorizonIX, DayOfWeek.Saturday, 16, 0),
        CreatePlan(
            28, StationIds.Istanbul, DestinationIds.Kepler452B, 840,
            SpaceshipIds.OdysseyPrime, DayOfWeek.Saturday, 18, 0),
        CreatePlan(
            29, StationIds.Berlin, DestinationIds.Kepler452B, 860,
            SpaceshipIds.StellarMeridian, DayOfWeek.Saturday, 20, 0),
        CreatePlan(
            30, StationIds.NewYork, DestinationIds.Kepler452B, 880,
            SpaceshipIds.ArtemisDawn, DayOfWeek.Saturday, 22, 0),
    ];

    // Projects plans into Route entities for later persistence.
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

    // Expands every route plan into outbound and return schedule entities.
    public static IReadOnlyList<WeeklySchedule> CreateWeeklySchedules() =>
        CreateRoutePlans()
            .SelectMany(CreateSchedulePair)
            .ToArray();

    // Builds both legs and derives return time after destination turnaround.
    private static IEnumerable<WeeklySchedule> CreateSchedulePair(
        WeeklyRoutePlan plan)
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

    // Combines reference IDs and timing inputs into one deterministic plan.
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

    // Produces stable entity IDs so repeated catalog creation is consistent.
    private static Guid CreateDeterministicId(int category, int sequence) =>
        Guid.Parse($"{category}0000000-0000-0000-0000-{sequence:D12}");

    // Converts schedule fields into a sortable Monday-based weekly offset.
    private static int ToMinuteOfWeek(DayOfWeek day, TimeOnly time)
    {
        var dayIndex = day == DayOfWeek.Sunday ? 6 : (int)day - 1;
        return (dayIndex * MinutesPerDay) + (time.Hour * 60) + time.Minute;
    }

    // Converts a weekly offset back to fields used by WeeklySchedule.
    private static (DayOfWeek Day, TimeOnly Time) FromMinuteOfWeek(int minute)
    {
        var normalizedMinute =
            ((minute % MinutesPerWeek) + MinutesPerWeek) % MinutesPerWeek;
        var dayIndex = normalizedMinute / MinutesPerDay;
        var minuteOfDay = normalizedMinute % MinutesPerDay;
        var day = dayIndex == 6 ? DayOfWeek.Sunday : (DayOfWeek)(dayIndex + 1);

        return (day, new TimeOnly(minuteOfDay / 60, minuteOfDay % 60));
    }
}
