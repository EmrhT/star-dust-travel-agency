using StarDustTravelAgency.Api.Domain.Enums;

namespace StarDustTravelAgency.Api.Domain.Entities;

public sealed class Destination
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DestinationType Type { get; set; }

    public string StarSystem { get; set; } = string.Empty;

    public string ShortDescription { get; set; } = string.Empty;

    public decimal GravityRelativeToEarth { get; set; }

    public decimal AverageTemperatureCelsius { get; set; }

    public string TravelAdvisory { get; set; } = string.Empty;

    public bool Active { get; set; } = true;

    public ICollection<Route> Routes { get; set; } = [];
}
