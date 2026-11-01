// Models one authenticated user's reservation and submitted booking details.
// BookingConfiguration persists its owner, form values, and Journey relation.
namespace StarDustTravelAgency.Api.Domain.Entities;

/// <summary>
/// Links one Keycloak subject and its form details to one Journey.
/// BookingConfiguration enforces ownership, value, and relation constraints.
/// </summary>
public sealed class Booking
{
    public Guid Id { get; set; }

    public Guid JourneyId { get; set; }

    public Journey Journey { get; set; } = null!;

    public string UserSubject { get; set; } = string.Empty;

    public int NumberOfTravelers { get; set; }

    public string ContactEmail { get; set; } = string.Empty;

    public string? SpecialRequests { get; set; }

    public DateTimeOffset BookedAtUtc { get; set; }
}
