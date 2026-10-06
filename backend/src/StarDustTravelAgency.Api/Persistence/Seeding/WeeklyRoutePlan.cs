namespace StarDustTravelAgency.Api.Persistence.Seeding;

public sealed record WeeklyRoutePlan(
    int Sequence,
    Guid RouteId,
    Guid LaunchStationId,
    Guid DestinationId,
    int DurationMinutes,
    Guid SpaceshipId,
    DayOfWeek OutboundDay,
    TimeOnly OutboundTime);
