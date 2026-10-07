using Microsoft.AspNetCore.Mvc;
using StarDustTravelAgency.Api.Services.Journeys;

// Exposes manual one-week journey generation through the REST API.
// It validates input and delegates persistence to JourneyGenerationService.
namespace StarDustTravelAgency.Api.Controllers;

/// <summary>
/// Handles journey read and generation requests for a Monday-based week.
/// Journey services provide generation and read-only weekly query operations.
/// </summary>
[ApiController]
[Route("api/journeys")]
public sealed class JourneysController(
    JourneyGenerationService generationService,
    JourneyQueryService queryService) : ControllerBase
{
    /// <summary>
    /// Returns JourneyQueryService results for one Monday-based week.
    /// </summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<JourneyListItem>>(
        StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(
        StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<JourneyListItem>>> GetWeek(
        [FromQuery] DateOnly? weekStarting,
        CancellationToken cancellationToken)
    {
        if (!ValidateWeekStarting(weekStarting))
        {
            return ValidationProblem(ModelState);
        }

        var journeys = await queryService.GetWeekAsync(
            weekStarting!.Value,
            cancellationToken);

        return Ok(journeys);
    }

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
        if (!ValidateWeekStarting(request.WeekStarting))
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

    /// <summary>
    /// Adds shared required-Monday errors for both controller actions.
    /// </summary>
    private bool ValidateWeekStarting(DateOnly? weekStarting)
    {
        if (weekStarting is null)
        {
            ModelState.AddModelError(
                "weekStarting",
                "weekStarting is required.");
        }
        else if (weekStarting.Value.DayOfWeek != DayOfWeek.Monday)
        {
            ModelState.AddModelError(
                "weekStarting",
                "weekStarting must be a Monday.");
        }

        return ModelState.IsValid;
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
