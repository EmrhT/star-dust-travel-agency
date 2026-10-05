namespace StarDustTravelAgency.Api.Domain.Entities;

public sealed class LaunchStation
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool Active { get; set; }

    public ICollection<Route> Routes { get; set; } = [];
}
