// Holds stable keys shared by reference records and timetable definitions.
// Deterministic values make repeated EF Core seed operations idempotent.
namespace StarDustTravelAgency.Api.Persistence.Seeding;

/// <summary>
/// Groups keys shared by ReferenceDataCatalog and WeeklyTimetableCatalog.
/// ReferenceDataSeeder relies on these stable keys for idempotency.
/// </summary>
public static class ReferenceDataIds
{
    /// <summary>
    /// Supplies keys to ReferenceDataCatalog.CreateLaunchStations.
    /// WeeklyTimetableCatalog uses them in each route plan.
    /// </summary>
    public static class LaunchStations
    {
        public static readonly Guid Tokyo =
            Guid.Parse("10000000-0000-0000-0000-000000000001");
        public static readonly Guid Istanbul =
            Guid.Parse("10000000-0000-0000-0000-000000000002");
        public static readonly Guid Berlin =
            Guid.Parse("10000000-0000-0000-0000-000000000003");
        public static readonly Guid NewYork =
            Guid.Parse("10000000-0000-0000-0000-000000000004");
    }

    /// <summary>
    /// Supplies keys to ReferenceDataCatalog.CreateDestinations.
    /// WeeklyTimetableCatalog uses them in each route plan.
    /// </summary>
    public static class Destinations
    {
        public static readonly Guid Mars =
            Guid.Parse("20000000-0000-0000-0000-000000000001");
        public static readonly Guid Venus =
            Guid.Parse("20000000-0000-0000-0000-000000000002");
        public static readonly Guid Titan =
            Guid.Parse("20000000-0000-0000-0000-000000000003");
        public static readonly Guid Europa =
            Guid.Parse("20000000-0000-0000-0000-000000000004");
        public static readonly Guid ProximaCentauriB =
            Guid.Parse("20000000-0000-0000-0000-000000000005");
        public static readonly Guid Trappist1E =
            Guid.Parse("20000000-0000-0000-0000-000000000006");
        public static readonly Guid Kepler186F =
            Guid.Parse("20000000-0000-0000-0000-000000000007");
        public static readonly Guid Ganymede =
            Guid.Parse("20000000-0000-0000-0000-000000000008");
        public static readonly Guid Enceladus =
            Guid.Parse("20000000-0000-0000-0000-000000000009");
        public static readonly Guid Kepler452B =
            Guid.Parse("20000000-0000-0000-0000-000000000010");
    }

    /// <summary>
    /// Supplies keys to ReferenceDataCatalog.CreateSpaceships.
    /// WeeklyTimetableCatalog uses them for schedule assignments.
    /// </summary>
    public static class Spaceships
    {
        public static readonly Guid CelestialNomad =
            Guid.Parse("30000000-0000-0000-0000-000000000001");
        public static readonly Guid AuroraVanguard =
            Guid.Parse("30000000-0000-0000-0000-000000000002");
        public static readonly Guid HorizonIX =
            Guid.Parse("30000000-0000-0000-0000-000000000003");
        public static readonly Guid OdysseyPrime =
            Guid.Parse("30000000-0000-0000-0000-000000000004");
        public static readonly Guid StellarMeridian =
            Guid.Parse("30000000-0000-0000-0000-000000000005");
        public static readonly Guid ArtemisDawn =
            Guid.Parse("30000000-0000-0000-0000-000000000006");
        public static readonly Guid SolarisAscendant =
            Guid.Parse("30000000-0000-0000-0000-000000000007");
        public static readonly Guid NebulaVoyager =
            Guid.Parse("30000000-0000-0000-0000-000000000008");
    }
}
