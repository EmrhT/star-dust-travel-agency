using StarDustTravelAgency.Api.Domain.Entities;
using StarDustTravelAgency.Api.Domain.Enums;

namespace StarDustTravelAgency.Api.Persistence.Seeding;

public static class ReferenceDataCatalog
{
    public static IReadOnlyList<LaunchStation> CreateLaunchStations() =>
    [
        new()
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
            Name = "Tokyo",
        },
        new()
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
            Name = "Istanbul",
        },
        new()
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
            Name = "Berlin",
        },
        new()
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000004"),
            Name = "New York",
        },
    ];

    public static IReadOnlyList<Destination> CreateDestinations() =>
    [
        new()
        {
            Id = Guid.Parse("20000000-0000-0000-0000-000000000001"),
            Name = "Mars",
            Type = DestinationType.Planet,
            StarSystem = "Solar System",
            ShortDescription = "A cold desert world with immense volcanoes, canyons, and a growing network of research settlements.",
            GravityRelativeToEarth = 0.380m,
            AverageTemperatureCelsius = -65m,
            TravelAdvisory = "Surface temperatures vary sharply; pressure suits are mandatory outside protected habitats.",
        },
        new()
        {
            Id = Guid.Parse("20000000-0000-0000-0000-000000000002"),
            Name = "Venus",
            Type = DestinationType.Planet,
            StarSystem = "Solar System",
            ShortDescription = "A cloud-covered terrestrial planet visited through temperate high-altitude stations.",
            GravityRelativeToEarth = 0.904m,
            AverageTemperatureCelsius = 464m,
            TravelAdvisory = "Surface excursions are prohibited; travelers remain within certified aerostat habitats.",
        },
        new()
        {
            Id = Guid.Parse("20000000-0000-0000-0000-000000000003"),
            Name = "Titan",
            Type = DestinationType.Moon,
            StarSystem = "Solar System",
            ShortDescription = "Saturn's largest moon, known for its dense atmosphere and hydrocarbon lakes.",
            GravityRelativeToEarth = 0.138m,
            AverageTemperatureCelsius = -179m,
            TravelAdvisory = "Extreme cold protection is required for all exterior activities.",
        },
        new()
        {
            Id = Guid.Parse("20000000-0000-0000-0000-000000000004"),
            Name = "Europa",
            Type = DestinationType.Moon,
            StarSystem = "Solar System",
            ShortDescription = "An ice-covered moon of Jupiter with a global ocean beneath its fractured surface.",
            GravityRelativeToEarth = 0.134m,
            AverageTemperatureCelsius = -160m,
            TravelAdvisory = "Radiation exposure is tightly controlled; remain inside shielded facilities.",
        },
        new()
        {
            Id = Guid.Parse("20000000-0000-0000-0000-000000000005"),
            Name = "Proxima Centauri b",
            Type = DestinationType.Exoplanet,
            StarSystem = "Proxima Centauri",
            ShortDescription = "The nearest known exoplanet to Earth, orbiting within the Proxima Centauri system.",
            GravityRelativeToEarth = 1.100m,
            AverageTemperatureCelsius = -39m,
            TravelAdvisory = "Climate and gravity values are agency planning estimates; follow local survey guidance.",
        },
        new()
        {
            Id = Guid.Parse("20000000-0000-0000-0000-000000000006"),
            Name = "TRAPPIST-1e",
            Type = DestinationType.Exoplanet,
            StarSystem = "TRAPPIST-1",
            ShortDescription = "A rocky, approximately Earth-sized world in the compact TRAPPIST-1 planetary system.",
            GravityRelativeToEarth = 0.820m,
            AverageTemperatureCelsius = -22m,
            TravelAdvisory = "Surface conditions remain under study; excursions require an approved environmental package.",
        },
        new()
        {
            Id = Guid.Parse("20000000-0000-0000-0000-000000000007"),
            Name = "Kepler-186f",
            Type = DestinationType.Exoplanet,
            StarSystem = "Kepler-186",
            ShortDescription = "An Earth-sized exoplanet completing the outermost known orbit in the Kepler-186 system.",
            GravityRelativeToEarth = 1.100m,
            AverageTemperatureCelsius = -85m,
            TravelAdvisory = "Atmospheric conditions are uncertain; independent surface travel is not permitted.",
        },
        new()
        {
            Id = Guid.Parse("20000000-0000-0000-0000-000000000008"),
            Name = "Ganymede",
            Type = DestinationType.Moon,
            StarSystem = "Solar System",
            ShortDescription = "Jupiter's largest moon and the largest natural satellite in the Solar System.",
            GravityRelativeToEarth = 0.146m,
            AverageTemperatureCelsius = -163m,
            TravelAdvisory = "Use radiation shelters during announced Jovian magnetosphere alerts.",
        },
        new()
        {
            Id = Guid.Parse("20000000-0000-0000-0000-000000000009"),
            Name = "Enceladus",
            Type = DestinationType.Moon,
            StarSystem = "Solar System",
            ShortDescription = "A small icy moon of Saturn whose south-polar plumes reveal a subsurface ocean.",
            GravityRelativeToEarth = 0.011m,
            AverageTemperatureCelsius = -201m,
            TravelAdvisory = "Very low gravity and plume activity require tethered exterior movement.",
        },
        new()
        {
            Id = Guid.Parse("20000000-0000-0000-0000-000000000010"),
            Name = "Kepler-452b",
            Type = DestinationType.Exoplanet,
            StarSystem = "Kepler-452",
            ShortDescription = "A super-Earth-size candidate world orbiting a Sun-like star on a long-period path.",
            GravityRelativeToEarth = 1.600m,
            AverageTemperatureCelsius = -8m,
            TravelAdvisory = "Properties remain uncertain; travelers must observe expedition-zone restrictions.",
        },
    ];

    public static IReadOnlyList<Spaceship> CreateSpaceships() =>
    [
        CreateSpaceship("30000000-0000-0000-0000-000000000001", "Celestial Nomad", "Asteria 180", 180, 2018),
        CreateSpaceship("30000000-0000-0000-0000-000000000002", "Aurora Vanguard", "Helios 220", 220, 2021),
        CreateSpaceship("30000000-0000-0000-0000-000000000003", "Horizon IX", "Horizon 140", 140, 2019),
        CreateSpaceship("30000000-0000-0000-0000-000000000004", "Odyssey Prime", "Odyssey 260", 260, 2024),
        CreateSpaceship("30000000-0000-0000-0000-000000000005", "Stellar Meridian", "Meridian 200", 200, 2020),
        CreateSpaceship("30000000-0000-0000-0000-000000000006", "Artemis Dawn", "Artemis 160", 160, 2022),
        CreateSpaceship("30000000-0000-0000-0000-000000000007", "Solaris Ascendant", "Solaris 240", 240, 2025),
        CreateSpaceship("30000000-0000-0000-0000-000000000008", "Nebula Voyager", "Nebula 190", 190, 2023),
    ];

    private static Spaceship CreateSpaceship(
        string id,
        string name,
        string model,
        int capacity,
        int yearBuilt) =>
        new()
        {
            Id = Guid.Parse(id),
            Name = name,
            Model = model,
            Capacity = capacity,
            YearBuilt = yearBuilt,
            Status = SpaceshipStatus.Available,
        };
}
