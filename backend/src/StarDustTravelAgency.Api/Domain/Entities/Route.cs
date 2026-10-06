// Models a station-to-destination path used by schedules and journeys.
// Persistence mappings enforce its relationships and duration constraints.
namespace StarDustTravelAgency.Api.Domain.Entities;

/// <summary>
/// Connects LaunchStation and Destination to schedules and journeys.
/// WeeklyTimetableCatalog creates it; RouteConfiguration maps it.
/// </summary>
public sealed class Route
{
    public Guid Id { get; set; }

    public Guid LaunchStationId { get; set; }

    public LaunchStation LaunchStation { get; set; } = null!;

    public Guid DestinationId { get; set; }

    public Destination Destination { get; set; } = null!;

    public int DurationMinutes { get; set; }

    public bool Active { get; set; } = true;

    public ICollection<WeeklySchedule> WeeklySchedules { get; set; } = [];

    public ICollection<Journey> Journeys { get; set; } = [];
}
