// Defines destination categories shared by domain records and EF conversion.
// DestinationConfiguration stores these named values in PostgreSQL.
namespace StarDustTravelAgency.Api.Domain.Enums;

/// <summary>
/// Categorizes Destination records created by ReferenceDataCatalog.
/// DestinationConfiguration converts its values to database strings.
/// </summary>
public enum DestinationType
{
    Planet,
    Moon,
    Exoplanet,
}
