using StarDustTravelAgency.Api.Persistence.Seeding;

namespace StarDustTravelAgency.Api.Tests;

public sealed class ReferenceDataCatalogTests
{
    [Fact]
    public void Catalog_contains_the_required_reference_records()
    {
        var launchStations = ReferenceDataCatalog.CreateLaunchStations();
        var destinations = ReferenceDataCatalog.CreateDestinations();
        var spaceships = ReferenceDataCatalog.CreateSpaceships();

        Assert.Equal(4, launchStations.Count);
        Assert.Equal(10, destinations.Count);
        Assert.Equal(8, spaceships.Count);

        Assert.Equal(
            ["Berlin", "Istanbul", "New York", "Tokyo"],
            launchStations.Select(station => station.Name).Order());

        Assert.Equal(launchStations.Count, launchStations.Select(station => station.Id).Distinct().Count());
        Assert.Equal(destinations.Count, destinations.Select(destination => destination.Id).Distinct().Count());
        Assert.Equal(spaceships.Count, spaceships.Select(spaceship => spaceship.Id).Distinct().Count());
    }
}
