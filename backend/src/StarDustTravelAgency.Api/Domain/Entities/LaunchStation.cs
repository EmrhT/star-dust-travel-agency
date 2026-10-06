// Models a terrestrial departure point used by routes and reference data.
// EF Core maps its route collection through RouteConfiguration.
namespace StarDustTravelAgency.Api.Domain.Entities;

/// <summary>
/// Supplies the departure side of each Route.
/// ReferenceDataCatalog creates it; its EF configuration maps it.
/// </summary>
public sealed class LaunchStation
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool Active { get; set; } = true;

    public ICollection<Route> Routes { get; set; } = [];
}
