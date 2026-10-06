using StarDustTravelAgency.Api.Domain.Enums;

// Models a bookable destination and its travel information for EF Core.
// Route links let scheduling and persistence associate trips with this place.
namespace StarDustTravelAgency.Api.Domain.Entities;

/// <summary>
/// Stores destination facts referenced by Route.
/// ReferenceDataCatalog creates it; DestinationConfiguration maps it.
/// </summary>
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
