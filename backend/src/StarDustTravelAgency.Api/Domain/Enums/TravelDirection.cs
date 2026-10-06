// Defines outbound and return legs shared by schedules and journeys.
// Their EF configurations store these named values in PostgreSQL.
namespace StarDustTravelAgency.Api.Domain.Enums;

/// <summary>
/// Distinguishes outbound and return WeeklySchedule and Journey legs.
/// Their EF configuration classes convert its values to strings.
/// </summary>
public enum TravelDirection
{
    Outbound,
    Return,
}
