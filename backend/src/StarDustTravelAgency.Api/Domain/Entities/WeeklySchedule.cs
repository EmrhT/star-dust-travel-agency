using StarDustTravelAgency.Api.Domain.Enums;

// Models one recurring weekly departure for a route and default spaceship.
// Timetable seed data creates these templates for future dated journeys.
namespace StarDustTravelAgency.Api.Domain.Entities;

/// <summary>
/// Links a recurring Route leg to its default Spaceship.
/// WeeklyTimetableCatalog creates it; its configuration maps it.
/// </summary>
public sealed class WeeklySchedule
{
    public Guid Id { get; set; }

    public Guid RouteId { get; set; }

    public Route Route { get; set; } = null!;

    public TravelDirection Direction { get; set; }

    public DayOfWeek DepartureDay { get; set; }

    public TimeOnly DepartureTime { get; set; }

    public Guid DefaultSpaceshipId { get; set; }

    public Spaceship DefaultSpaceship { get; set; } = null!;

    public bool Active { get; set; } = true;

    public ICollection<Journey> Journeys { get; set; } = [];
}
