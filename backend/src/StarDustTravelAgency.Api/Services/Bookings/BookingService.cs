using Microsoft.EntityFrameworkCore;
using StarDustTravelAgency.Api.Domain.Entities;
using StarDustTravelAgency.Api.Domain.Enums;
using StarDustTravelAgency.Api.Persistence;

// Creates identity-owned Booking records against existing scheduled Journeys.
// BookingsController supplies the trusted subject read from the validated JWT.
namespace StarDustTravelAgency.Api.Services.Bookings;

/// <summary>
/// Validates Journey state and duplicate ownership before persisting a Booking.
/// BookingConfiguration remains the final database-integrity boundary.
/// </summary>
public sealed class BookingService(
    ApplicationDbContext context)
{
    /// <summary>
    /// Creates one Booking for the trusted subject and requested Journey.
    /// BookingsController maps its outcome to an HTTP response.
    /// </summary>
    public async Task<BookingCreationResult> CreateAsync(
        string userSubject,
        Guid journeyId,
        CancellationToken cancellationToken = default)
    {
        var journeyStatus = await context.Journeys
            .AsNoTracking()
            .Where(journey => journey.Id == journeyId)
            .Select(journey => (JourneyStatus?)journey.Status)
            .SingleOrDefaultAsync(cancellationToken);

        if (!journeyStatus.HasValue)
        {
            return BookingCreationResult.WithOutcome(
                BookingCreationOutcome.JourneyNotFound);
        }

        if (journeyStatus.Value != JourneyStatus.Scheduled)
        {
            return BookingCreationResult.WithOutcome(
                BookingCreationOutcome.JourneyUnavailable);
        }

        var alreadyExists = await context.Bookings
            .AsNoTracking()
            .AnyAsync(
                booking =>
                    booking.UserSubject == userSubject
                    && booking.JourneyId == journeyId,
                cancellationToken);

        if (alreadyExists)
        {
            return BookingCreationResult.WithOutcome(
                BookingCreationOutcome.AlreadyExists);
        }

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            JourneyId = journeyId,
            UserSubject = userSubject,
            BookedAtUtc = DateTimeOffset.UtcNow,
        };

        context.Bookings.Add(booking);
        await context.SaveChangesAsync(cancellationToken);

        return new BookingCreationResult(
            BookingCreationOutcome.Created,
            booking);
    }
}

/// <summary>
/// Names expected BookingService outcomes for BookingsController to translate.
/// Database or infrastructure failures remain exceptions handled globally.
/// </summary>
public enum BookingCreationOutcome
{
    Created,
    JourneyNotFound,
    JourneyUnavailable,
    AlreadyExists,
}

/// <summary>
/// Carries a Booking only when BookingService successfully creates one.
/// BookingsController uses Outcome to select the corresponding HTTP status.
/// </summary>
public sealed record BookingCreationResult(
    BookingCreationOutcome Outcome,
    Booking? Booking = null)
{
    /// <summary>
    /// Creates a result for an expected non-success service outcome.
    /// </summary>
    public static BookingCreationResult WithOutcome(
        BookingCreationOutcome outcome) =>
        new(outcome);
}
