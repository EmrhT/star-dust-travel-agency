// Defines fleet availability states used by Spaceship and reference data.
// SpaceshipConfiguration stores these named values in PostgreSQL.
namespace StarDustTravelAgency.Api.Domain.Enums;

/// <summary>
/// Describes fleet availability on each Spaceship.
/// ReferenceDataCatalog assigns it; SpaceshipConfiguration stores it.
/// </summary>
public enum SpaceshipStatus
{
    Available,
    InService,
    Maintenance,
    Retired,
}
