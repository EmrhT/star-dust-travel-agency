// Centralizes the UTC+3 business-time convention used by journey services.
// Query and generation paths share these conversions before persistence.
namespace StarDustTravelAgency.Api.Domain.Time;

/// <summary>
/// Converts business-local dates and times into stored UTC timestamps.
/// JourneyQueryService and JourneyGenerationService call this shared policy.
/// </summary>
public static class BusinessTime
{
    private static readonly TimeSpan UtcPlusThree = TimeSpan.FromHours(3);

    /// <summary>
    /// Gives JourneyQueryService a UTC boundary for a business-local date.
    /// </summary>
    public static DateTimeOffset StartOfDayUtc(DateOnly date) =>
        ToUtc(date, TimeOnly.MinValue);

    /// <summary>
    /// Gives JourneyGenerationService a UTC instant for a local schedule time.
    /// </summary>
    public static DateTimeOffset ToUtc(DateOnly date, TimeOnly time)
    {
        var localDateTime = date.ToDateTime(time);
        return new DateTimeOffset(localDateTime, UtcPlusThree)
            .ToUniversalTime();
    }
}
