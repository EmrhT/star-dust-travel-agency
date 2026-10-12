using StarDustTravelAgency.Api.Domain.Time;

// Verifies the shared UTC+3 conversions used by both journey services.
// These tests prevent query and generation boundaries from drifting apart.
namespace StarDustTravelAgency.Api.Tests;

/// <summary>
/// Exercises BusinessTime with start-of-day and scheduled-time inputs.
/// Journey services depend on these values for matching stored occurrences.
/// </summary>
public sealed class BusinessTimeTests
{
    /// <summary>
    /// Verifies StartOfDayUtc converts UTC+3 midnight to the prior UTC date.
    /// </summary>
    [Fact]
    public void Start_of_day_uses_the_utc_plus_three_business_offset()
    {
        var result = BusinessTime.StartOfDayUtc(
            new DateOnly(2026, 10, 12));

        Assert.Equal(
            new DateTimeOffset(2026, 10, 11, 21, 0, 0, TimeSpan.Zero),
            result);
    }

    /// <summary>
    /// Verifies ToUtc preserves minutes while removing the UTC+3 offset.
    /// </summary>
    [Fact]
    public void Scheduled_time_uses_the_utc_plus_three_business_offset()
    {
        var result = BusinessTime.ToUtc(
            new DateOnly(2026, 10, 12),
            new TimeOnly(8, 30));

        Assert.Equal(
            new DateTimeOffset(2026, 10, 12, 5, 30, 0, TimeSpan.Zero),
            result);
    }
}
