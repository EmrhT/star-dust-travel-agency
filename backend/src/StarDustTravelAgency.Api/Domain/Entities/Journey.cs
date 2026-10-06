using StarDustTravelAgency.Api.Domain.Enums;

// Models one dated trip created from route, schedule, and spaceship data.
// EF Core mappings persist these relations for later API workflows.
namespace StarDustTravelAgency.Api.Domain.Entities;

/// <summary>
/// Links a dated trip to Route, WeeklySchedule, and Spaceship.
/// JourneyConfiguration maps these relations into PostgreSQL.
/// </summary>
public sealed class Journey
{
    public Guid Id { get; set; }

    public Guid RouteId { get; set; }

    public Route Route { get; set; } = null!;

    public Guid? WeeklyScheduleId { get; set; }

    public WeeklySchedule? WeeklySchedule { get; set; }

    public TravelDirection Direction { get; set; }

    public DateTimeOffset DepartureAtUtc { get; set; }

    public DateTimeOffset ArrivalAtUtc { get; set; }

    public Guid SpaceshipId { get; set; }

    public Spaceship Spaceship { get; set; } = null!;

    public JourneyStatus Status { get; set; }
}
