using StarDustTravelAgency.Api.Domain.Entities;
using StarDustTravelAgency.Api.Domain.Enums;

namespace StarDustTravelAgency.Api.Persistence.Seeding;

public static class ReferenceDataCatalog
{
    public static IReadOnlyList<LaunchStation> CreateLaunchStations() =>
    [
        new()
        {
            Id = ReferenceDataIds.LaunchStations.Tokyo,
            Name = "Tokyo",
        },
        new()
        {
            Id = ReferenceDataIds.LaunchStations.Istanbul,
            Name = "Istanbul",
        },
        new()
        {
            Id = ReferenceDataIds.LaunchStations.Berlin,
            Name = "Berlin",
        },
        new()
        {
            Id = ReferenceDataIds.LaunchStations.NewYork,
            Name = "New York",
        },
    ];

    public static IReadOnlyList<Destination> CreateDestinations() =>
    [
        new()
        {
            Id = ReferenceDataIds.Destinations.Mars,
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
            Id = ReferenceDataIds.Destinations.Venus,
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
            Id = ReferenceDataIds.Destinations.Titan,
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
            Id = ReferenceDataIds.Destinations.Europa,
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
            Id = ReferenceDataIds.Destinations.ProximaCentauriB,
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
            Id = ReferenceDataIds.Destinations.Trappist1E,
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
            Id = ReferenceDataIds.Destinations.Kepler186F,
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
            Id = ReferenceDataIds.Destinations.Ganymede,
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
            Id = ReferenceDataIds.Destinations.Enceladus,
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
            Id = ReferenceDataIds.Destinations.Kepler452B,
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
        CreateSpaceship(ReferenceDataIds.Spaceships.CelestialNomad, "Celestial Nomad", "Asteria 180", 180, 2018),
        CreateSpaceship(ReferenceDataIds.Spaceships.AuroraVanguard, "Aurora Vanguard", "Helios 220", 220, 2021),
        CreateSpaceship(ReferenceDataIds.Spaceships.HorizonIX, "Horizon IX", "Horizon 140", 140, 2019),
        CreateSpaceship(ReferenceDataIds.Spaceships.OdysseyPrime, "Odyssey Prime", "Odyssey 260", 260, 2024),
        CreateSpaceship(ReferenceDataIds.Spaceships.StellarMeridian, "Stellar Meridian", "Meridian 200", 200, 2020),
        CreateSpaceship(ReferenceDataIds.Spaceships.ArtemisDawn, "Artemis Dawn", "Artemis 160", 160, 2022),
        CreateSpaceship(ReferenceDataIds.Spaceships.SolarisAscendant, "Solaris Ascendant", "Solaris 240", 240, 2025),
        CreateSpaceship(ReferenceDataIds.Spaceships.NebulaVoyager, "Nebula Voyager", "Nebula 190", 190, 2023),
    ];

    private static Spaceship CreateSpaceship(
        Guid id,
        string name,
        string model,
        int capacity,
        int yearBuilt) =>
        new()
        {
            Id = id,
            Name = name,
            Model = model,
            Capacity = capacity,
            YearBuilt = yearBuilt,
            Status = SpaceshipStatus.Available,
        };
}
