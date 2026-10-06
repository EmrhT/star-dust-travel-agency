// Defines the intermediate plan used to build routes and paired schedules.
// WeeklyTimetableCatalog turns each plan into persisted domain entities.
namespace StarDustTravelAgency.Api.Persistence.Seeding;

/// <summary>
/// Carries ReferenceDataIds and timing inputs for one route.
/// WeeklyTimetableCatalog projects it into Route and WeeklySchedule.
/// </summary>
public sealed record WeeklyRoutePlan(
    int Sequence,
    Guid RouteId,
    Guid LaunchStationId,
    Guid DestinationId,
    int DurationMinutes,
    Guid SpaceshipId,
    DayOfWeek OutboundDay,
    TimeOnly OutboundTime);
