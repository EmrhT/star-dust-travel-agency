// Defines the operational lifecycle states available to a Journey.
// JourneyConfiguration stores these named values in PostgreSQL.
namespace StarDustTravelAgency.Api.Domain.Enums;

/// <summary>
/// Describes a Journey's operational lifecycle state.
/// JourneyConfiguration converts its values to database strings.
/// </summary>
public enum JourneyStatus
{
    Draft,
    Scheduled,
    Boarding,
    Departed,
    Arrived,
    Delayed,
    Cancelled,
}
