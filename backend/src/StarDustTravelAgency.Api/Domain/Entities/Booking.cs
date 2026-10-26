// Models one authenticated user's reservation of an existing Journey.
// BookingConfiguration persists its owner, timestamp, and Journey relation.
namespace StarDustTravelAgency.Api.Domain.Entities;

/// <summary>
/// Links one Keycloak subject to one Journey without duplicating user data.
/// BookingConfiguration enforces ownership and relationship constraints.
/// </summary>
public sealed class Booking
{
    public Guid Id { get; set; }

    public Guid JourneyId { get; set; }

    public Journey Journey { get; set; } = null!;

    public string UserSubject { get; set; } = string.Empty;

    public DateTimeOffset BookedAtUtc { get; set; }
}
