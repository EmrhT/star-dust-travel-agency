using StarDustTravelAgency.Api.Domain.Enums;

// Models a vehicle assigned to recurring schedules and dated journeys.
// Seed catalogs create the fleet and EF Core persists these relations.
namespace StarDustTravelAgency.Api.Domain.Entities;

/// <summary>
/// Supplies the vehicle assigned to WeeklySchedule and Journey.
/// ReferenceDataCatalog creates it; SpaceshipConfiguration maps it.
/// </summary>
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
