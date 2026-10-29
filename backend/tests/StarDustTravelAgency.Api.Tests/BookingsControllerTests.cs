using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarDustTravelAgency.Api.Controllers;
using StarDustTravelAgency.Api.Persistence;
using StarDustTravelAgency.Api.Services.Bookings;

// Verifies BookingsController rejects untrusted ownership in both operations.
// A non-connecting DbContext keeps these controller-boundary tests isolated.
namespace StarDustTravelAgency.Api.Tests;

/// <summary>
/// Exercises identity and request validation before BookingService queries EF.
/// OpenApiContractTests separately protect the public endpoint description.
/// </summary>
public sealed class BookingsControllerTests
{
    /// <summary>
    /// Confirms a subjectless identity cannot query another user's Bookings.
    /// </summary>
    [Fact]
    public async Task GetMine_forbids_identity_without_subject()
    {
        await using var context = CreateContext();
        var controller = CreateController(
            context,
            new Claim("preferred_username", "subjectless-user"));

        var result = await controller.GetMine(CancellationToken.None);

        Assert.IsType<ForbidResult>(result.Result);
    }

    /// <summary>
    /// Confirms an authenticated identity without sub cannot own a Booking.
    /// </summary>
    [Fact]
    public async Task Create_forbids_identity_without_subject()
    {
        await using var context = CreateContext();
        var controller = CreateController(
            context,
            new Claim("preferred_username", "subjectless-user"));

        var result = await controller.Create(
            new CreateBookingRequest
            {
                JourneyId = Guid.NewGuid(),
            },
            CancellationToken.None);

        Assert.IsType<ForbidResult>(result.Result);
    }

    /// <summary>
    /// Creates controller dependencies without opening a PostgreSQL connection.
    /// </summary>
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(
                "Host=localhost;Database=controller_test;" +
                "Username=test;Password=test")
            .Options;

        return new ApplicationDbContext(options);
    }

    /// <summary>
    /// Assigns an authenticated ClaimsPrincipal to a new BookingsController.
    /// </summary>
    private static BookingsController CreateController(
        ApplicationDbContext context,
        params Claim[] claims)
    {
        var identity = new ClaimsIdentity(claims, "test");

        return new BookingsController(new BookingService(context))
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(identity),
                },
            },
        };
    }
}
