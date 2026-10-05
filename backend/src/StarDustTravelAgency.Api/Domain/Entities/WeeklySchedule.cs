using StarDustTravelAgency.Api.Domain.Enums;

namespace StarDustTravelAgency.Api.Domain.Entities;

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

    public bool Active { get; set; }

    public ICollection<Journey> Journeys { get; set; } = [];
}
