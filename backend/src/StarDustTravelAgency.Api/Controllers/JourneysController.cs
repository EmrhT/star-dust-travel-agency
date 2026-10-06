using Microsoft.AspNetCore.Mvc;
using StarDustTravelAgency.Api.Services.Journeys;

// Exposes manual one-week journey generation through the REST API.
// It validates input and delegates persistence to JourneyGenerationService.
namespace StarDustTravelAgency.Api.Controllers;

/// <summary>
/// Handles journey-generation requests for one Monday-based schedule week.
/// JourneyGenerationService performs the time conversion and persistence.
/// </summary>
[ApiController]
[Route("api/journeys")]
public sealed class JourneysController(
    JourneyGenerationService generationService) : ControllerBase
{
    /// <summary>
    /// Validates the requested Monday and delegates one week of generation.
    /// </summary>
    [HttpPost("generate-week")]
    [ProducesResponseType<GenerateJourneyWeekResponse>(
        StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(
        StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GenerateJourneyWeekResponse>> GenerateWeek(
        GenerateJourneyWeekRequest request,
        CancellationToken cancellationToken)
    {
        if (request.WeekStarting is null)
        {
            ModelState.AddModelError(
                nameof(request.WeekStarting),
                "weekStarting is required.");
        }
        else if (request.WeekStarting.Value.DayOfWeek != DayOfWeek.Monday)
        {
            ModelState.AddModelError(
                nameof(request.WeekStarting),
                "weekStarting must be a Monday.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var result = await generationService.GenerateWeekAsync(
            request.WeekStarting!.Value,
            cancellationToken);

        return Ok(new GenerateJourneyWeekResponse(
            request.WeekStarting.Value,
            result.Created,
            result.AlreadyExisted));
    }
}

/// <summary>
/// Describes the Monday for JourneysController to generate.
/// React will serialize its nullable date as the API request body.
/// </summary>
public sealed record GenerateJourneyWeekRequest(DateOnly? WeekStarting);

/// <summary>
/// Reports JourneyGenerationService results to the requesting client.
/// ASP.NET Core serializes this record into the endpoint's JSON response.
/// </summary>
public sealed record GenerateJourneyWeekResponse(
    DateOnly WeekStarting,
    int Created,
    int AlreadyExisted);
