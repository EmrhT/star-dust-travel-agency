namespace StarDustTravelAgency.Api.Domain.Entities;

public sealed class Route
{
    public Guid Id { get; set; }

    public Guid LaunchStationId { get; set; }

    public LaunchStation LaunchStation { get; set; } = null!;

    public Guid DestinationId { get; set; }

    public Destination Destination { get; set; } = null!;

    public int DurationMinutes { get; set; }

    public bool Active { get; set; }

    public ICollection<WeeklySchedule> WeeklySchedules { get; set; } = [];

    public ICollection<Journey> Journeys { get; set; } = [];
}
