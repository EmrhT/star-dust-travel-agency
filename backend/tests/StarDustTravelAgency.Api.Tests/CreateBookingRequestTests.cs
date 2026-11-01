using System.ComponentModel.DataAnnotations;
using StarDustTravelAgency.Api.Controllers;

// Verifies browser booking fields against the controller's public contract.
// ASP.NET Core uses these annotations before invoking BookingsController.
namespace StarDustTravelAgency.Api.Tests;

/// <summary>
/// Exercises CreateBookingRequest rules without requiring PostgreSQL.
/// OpenApiContractTests separately protects their generated required fields.
/// </summary>
public sealed class CreateBookingRequestTests
{
    [Fact]
    // Accepts the smallest complete form BookingsController can handle.
    public void Complete_request_passes_validation()
    {
        var request = CreateValidRequest();

        var results = Validate(request);

        Assert.Empty(results);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(7)]
    // Rejects counts outside BookingConfiguration's database check range.
    public void Traveler_count_outside_range_fails_validation(
        int numberOfTravelers)
    {
        var request = CreateValidRequest() with
        {
            NumberOfTravelers = numberOfTravelers,
        };

        var results = Validate(request);

        Assert.Contains(
            results,
            result => result.MemberNames.Contains("NumberOfTravelers"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    // Rejects contact text that cannot represent an email address.
    public void Invalid_contact_email_fails_validation(string contactEmail)
    {
        var request = CreateValidRequest() with
        {
            ContactEmail = contactEmail,
        };

        var results = Validate(request);

        Assert.Contains(
            results,
            result => result.MemberNames.Contains("ContactEmail"));
    }

    [Fact]
    // Rejects optional notes whose length exceeds the database mapping.
    public void Oversized_special_requests_fail_validation()
    {
        var request = CreateValidRequest() with
        {
            SpecialRequests = new string('a', 501),
        };

        var results = Validate(request);

        Assert.Contains(
            results,
            result => result.MemberNames.Contains("SpecialRequests"));
    }

    /// <summary>
    /// Builds a request shared by focused validation variations above.
    /// </summary>
    private static CreateBookingRequest CreateValidRequest() =>
        new()
        {
            JourneyId = Guid.NewGuid(),
            NumberOfTravelers = 2,
            ContactEmail = "traveler@example.com",
            SpecialRequests = "Window seats if possible.",
        };

    /// <summary>
    /// Runs the same DataAnnotations consumed by ASP.NET Core model validation.
    /// </summary>
    private static IReadOnlyList<ValidationResult> Validate(
        CreateBookingRequest request)
    {
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(
            request,
            new ValidationContext(request),
            results,
            validateAllProperties: true);

        return results;
    }
}
