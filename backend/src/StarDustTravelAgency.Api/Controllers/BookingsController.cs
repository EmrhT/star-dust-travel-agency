using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using StarDustTravelAgency.Api.Domain.Entities;
using StarDustTravelAgency.Api.Services.Bookings;

// Exposes authenticated Booking creation without accepting browser ownership.
// It reads the validated JWT subject and delegates to BookingService.
namespace StarDustTravelAgency.Api.Controllers;

/// <summary>
/// Creates Bookings owned by the authenticated Keycloak subject.
/// BookingService validates the requested Journey and persists the record.
/// </summary>
[Authorize]
[ApiController]
[Route("api/bookings")]
public sealed class BookingsController(
    BookingService bookingService) : ControllerBase
{
    /// <summary>
    /// Derives the owner from JWT sub and asks BookingService to create a row.
    /// </summary>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType<CreateBookingResponse>(
        StatusCodes.Status201Created,
        "application/json")]
    [ProducesResponseType<ValidationProblemDetails>(
        StatusCodes.Status400BadRequest,
        "application/problem+json")]
    [ProducesResponseType(
        typeof(void),
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(void),
        StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(
        StatusCodes.Status404NotFound,
        "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(
        StatusCodes.Status409Conflict,
        "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(
        StatusCodes.Status500InternalServerError,
        "application/problem+json")]
    public async Task<ActionResult<CreateBookingResponse>> Create(
        [FromBody, BindRequired] CreateBookingRequest request,
        CancellationToken cancellationToken)
    {
        var userSubject = User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(userSubject))
        {
            return Forbid();
        }

        if (request.JourneyId == Guid.Empty)
        {
            ModelState.AddModelError(
                "journeyId",
                "journeyId must be a non-empty UUID.");

            return ValidationProblem(ModelState);
        }

        var result = await bookingService.CreateAsync(
            userSubject,
            request.JourneyId,
            cancellationToken);

        return ToActionResult(result);
    }

    /// <summary>
    /// Maps BookingService outcomes to the public HTTP booking contract.
    /// </summary>
    private ActionResult<CreateBookingResponse> ToActionResult(
        BookingCreationResult result)
    {
        return result.Outcome switch
        {
            BookingCreationOutcome.Created => StatusCode(
                StatusCodes.Status201Created,
                ToResponse(result.Booking!)),
            BookingCreationOutcome.JourneyNotFound => Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Journey not found.",
                detail: "The requested journey does not exist."),
            BookingCreationOutcome.JourneyUnavailable => Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Journey is unavailable.",
                detail: "Only scheduled journeys can be booked."),
            BookingCreationOutcome.AlreadyExists => Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Booking already exists.",
                detail: "You have already booked this journey."),
            _ => throw new InvalidOperationException(
                $"Unsupported booking outcome: {result.Outcome}"),
        };
    }

    /// <summary>
    /// Projects the persisted Booking without exposing its owner subject.
    /// </summary>
    private static CreateBookingResponse ToResponse(
        Booking booking) =>
        new(
            booking.Id,
            booking.JourneyId,
            booking.BookedAtUtc);
}

/// <summary>
/// Carries only the browser-selected Journey into BookingsController.
/// The authenticated ClaimsPrincipal supplies ownership separately.
/// </summary>
public sealed record CreateBookingRequest
{
    /// <summary>
    /// Identifies the Journey passed to BookingService for validation.
    /// </summary>
    public required Guid JourneyId { get; init; }
}

/// <summary>
/// Reports the created Booking without exposing its Keycloak subject.
/// ASP.NET Core serializes it for the generated frontend client.
/// </summary>
public sealed record CreateBookingResponse(
    Guid Id,
    Guid JourneyId,
    DateTimeOffset BookedAtUtc);
