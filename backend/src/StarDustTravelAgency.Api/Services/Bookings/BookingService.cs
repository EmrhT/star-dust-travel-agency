using Microsoft.EntityFrameworkCore;
using StarDustTravelAgency.Api.Domain.Entities;
using StarDustTravelAgency.Api.Domain.Enums;
using StarDustTravelAgency.Api.Persistence;

// Creates and queries identity-owned Bookings against scheduled Journeys.
// BookingsController supplies the trusted subject read from the validated JWT.
namespace StarDustTravelAgency.Api.Services.Bookings;

/// <summary>
/// Persists valid Bookings and projects those owned by one trusted subject.
/// BookingsController translates its results into the public HTTP contract.
/// </summary>
public sealed class BookingService(
    ApplicationDbContext context)
{
    /// <summary>
    /// Queries and projects only Bookings owned by the trusted JWT subject.
    /// BookingsController returns these display-ready rows to the browser.
    /// </summary>
    public async Task<IReadOnlyList<BookingListItem>> GetMineAsync(
        string userSubject,
        CancellationToken cancellationToken = default)
    {
        return await context.Bookings
            .AsNoTracking()
            .Where(booking => booking.UserSubject == userSubject)
            .OrderByDescending(booking => booking.BookedAtUtc)
            .Select(booking => new BookingListItem(
                booking.Id,
                booking.JourneyId,
                booking.BookedAtUtc,
                booking.Journey.Route.LaunchStation.Name,
                booking.Journey.Route.Destination.Name,
                booking.Journey.DepartureAtUtc,
                booking.Journey.ArrivalAtUtc,
                booking.Journey.Spaceship.Name,
                booking.Journey.Status))
            .ToArrayAsync(cancellationToken);
    }

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
/// Projects one owned Booking with Journey details needed by the frontend.
/// BookingService creates it without exposing the owner's Keycloak subject.
/// </summary>
public sealed record BookingListItem(
    Guid Id,
    Guid JourneyId,
    DateTimeOffset BookedAtUtc,
    string LaunchStation,
    string Destination,
    DateTimeOffset DepartureAtUtc,
    DateTimeOffset ArrivalAtUtc,
    string Spaceship,
    JourneyStatus Status);

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
