using StarDustTravelAgency.Api.Domain.Enums;

namespace StarDustTravelAgency.Api.Domain.Entities;

public sealed class Spaceship
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public int YearBuilt { get; set; }

    public SpaceshipStatus Status { get; set; }

    public ICollection<WeeklySchedule> WeeklySchedules { get; set; } = [];

    public ICollection<Journey> Journeys { get; set; } = [];
}
