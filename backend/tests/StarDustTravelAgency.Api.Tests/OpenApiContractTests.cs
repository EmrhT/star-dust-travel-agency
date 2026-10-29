using System.Text.Json;

// Guards the OpenAPI details consumed by the generated TypeScript client.
// The test factory exposes Swagger without PostgreSQL or authentication.
namespace StarDustTravelAgency.Api.Tests;

/// <summary>
/// Verifies Program and controller metadata produce a stable client contract.
/// The assertions protect operation names, inputs, responses, and DTO schemas.
/// </summary>
public sealed class OpenApiContractTests
    : IClassFixture<UnavailablePostgresApiFactory>
{
    private readonly HttpClient _client;

    // Creates a client for the Swagger endpoint hosted by the test factory.
    public OpenApiContractTests(UnavailablePostgresApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    /// <summary>
    /// Protects NSwag method names and required request inputs.
    /// </summary>
    [Fact]
    public async Task Operations_have_stable_ids_and_required_inputs()
    {
        using var document = await GetDocumentAsync();
        var getWeek = GetOperation(document, "/api/journeys", "get");
        var generateWeek = GetOperation(
            document,
            "/api/journeys/generate-week",
            "post");
        var createBooking = GetOperation(
            document,
            "/api/bookings",
            "post");
        var getMyBookings = GetOperation(
            document,
            "/api/bookings/mine",
            "get");
        var getStatus = GetOperation(
            document,
            "/api/system/status",
            "get");

        Assert.Equal(
            "Journeys_GetWeek",
            getWeek.GetProperty("operationId").GetString());
        Assert.Equal(
            "Journeys_GenerateWeek",
            generateWeek.GetProperty("operationId").GetString());
        Assert.Equal(
            "System_GetStatus",
            getStatus.GetProperty("operationId").GetString());
        Assert.Equal(
            "Bookings_Create",
            createBooking.GetProperty("operationId").GetString());
        Assert.Equal(
            "Bookings_GetMine",
            getMyBookings.GetProperty("operationId").GetString());

        var weekStarting = getWeek
            .GetProperty("parameters")
            .EnumerateArray()
            .Single(parameter =>
                parameter.GetProperty("name").GetString()
                == "weekStarting");

        Assert.True(weekStarting.GetProperty("required").GetBoolean());
        Assert.True(
            generateWeek
                .GetProperty("requestBody")
                .GetProperty("required")
                .GetBoolean());
        Assert.True(
            createBooking
                .GetProperty("requestBody")
                .GetProperty("required")
                .GetBoolean());
    }

    /// <summary>
    /// Protects success JSON and RFC 7807 error media-type declarations.
    /// </summary>
    [Fact]
    public async Task Responses_declare_exact_media_types()
    {
        using var document = await GetDocumentAsync();
        var getWeek = GetOperation(document, "/api/journeys", "get");
        var generateWeek = GetOperation(
            document,
            "/api/journeys/generate-week",
            "post");
        var getStatus = GetOperation(
            document,
            "/api/system/status",
            "get");
        var createBooking = GetOperation(
            document,
            "/api/bookings",
            "post");
        var getMyBookings = GetOperation(
            document,
            "/api/bookings/mine",
            "get");

        AssertResponseContentType(getWeek, "200", "application/json");
        AssertResponseContentType(
            getWeek,
            "400",
            "application/problem+json");
        AssertResponseContentType(
            getWeek,
            "500",
            "application/problem+json");
        AssertResponseContentType(
            generateWeek,
            "200",
            "application/json");
        AssertResponseContentType(
            generateWeek,
            "400",
            "application/problem+json");
        AssertResponseContentType(
            generateWeek,
            "500",
            "application/problem+json");
        AssertResponseContentType(getStatus, "200", "application/json");
        AssertResponseContentType(
            getStatus,
            "500",
            "application/problem+json");
        AssertResponseContentType(
            createBooking,
            "201",
            "application/json");
        AssertResponseContentType(
            createBooking,
            "400",
            "application/problem+json");
        AssertResponseContentType(
            createBooking,
            "404",
            "application/problem+json");
        AssertResponseContentType(
            createBooking,
            "409",
            "application/problem+json");
        AssertResponseContentType(
            createBooking,
            "500",
            "application/problem+json");
        AssertResponseContentType(
            getMyBookings,
            "200",
            "application/json");
        AssertResponseContentType(
            getMyBookings,
            "500",
            "application/problem+json");

        var requestContent = generateWeek
            .GetProperty("requestBody")
            .GetProperty("content")
            .EnumerateObject()
            .Single();
        var bookingRequestContent = createBooking
            .GetProperty("requestBody")
            .GetProperty("content")
            .EnumerateObject()
            .Single();

        Assert.Equal("application/json", requestContent.Name);
        Assert.Equal("application/json", bookingRequestContent.Name);
    }

    /// <summary>
    /// Protects required DTO members used to generate non-optional TS fields.
    /// </summary>
    [Fact]
    public async Task Schemas_require_non_nullable_members()
    {
        using var document = await GetDocumentAsync();

        AssertRequiredProperties(
            document,
            "CreateBookingRequest",
            "journeyId");
        AssertRequiredProperties(
            document,
            "CreateBookingResponse",
            "id",
            "journeyId",
            "bookedAtUtc");
        AssertRequiredProperties(
            document,
            "BookingListItem",
            "id",
            "journeyId",
            "bookedAtUtc",
            "launchStation",
            "destination",
            "departureAtUtc",
            "arrivalAtUtc",
            "spaceship",
            "status");
        AssertRequiredProperties(
            document,
            "GenerateJourneyWeekRequest",
            "weekStarting");
        AssertRequiredProperties(
            document,
            "GenerateJourneyWeekResponse",
            "weekStarting",
            "created",
            "alreadyExisted");
        AssertRequiredProperties(
            document,
            "JourneyListItem",
            "id",
            "launchStation",
            "destination",
            "direction",
            "departureAtUtc",
            "arrivalAtUtc",
            "spaceship",
            "status");
        AssertRequiredProperties(
            document,
            "SystemStatusResponse",
            "service",
            "database",
            "checkedAtUtc");
    }

    /// <summary>
    /// Protects the bearer scheme and the documented denial responses.
    /// </summary>
    [Fact]
    public async Task Authentication_contract_declares_bearer_and_denials()
    {
        using var document = await GetDocumentAsync();
        var root = document.RootElement;
        var bearer = root
            .GetProperty("components")
            .GetProperty("securitySchemes")
            .GetProperty("Bearer");
        var getWeek = GetOperation(document, "/api/journeys", "get");
        var generateWeek = GetOperation(
            document,
            "/api/journeys/generate-week",
            "post");
        var createBooking = GetOperation(
            document,
            "/api/bookings",
            "post");
        var getMyBookings = GetOperation(
            document,
            "/api/bookings/mine",
            "get");

        Assert.Equal("http", bearer.GetProperty("type").GetString());
        Assert.Equal("bearer", bearer.GetProperty("scheme").GetString());
        Assert.True(
            root.GetProperty("security")[0]
                .TryGetProperty("Bearer", out _));
        AssertEmptyResponse(getWeek, "401");
        AssertEmptyResponse(generateWeek, "401");
        AssertEmptyResponse(generateWeek, "403");
        AssertEmptyResponse(createBooking, "401");
        AssertEmptyResponse(createBooking, "403");
        AssertEmptyResponse(getMyBookings, "401");
        AssertEmptyResponse(getMyBookings, "403");
    }

    /// <summary>
    /// Reads Program's Swagger JSON for each contract-focused assertion.
    /// </summary>
    private async Task<JsonDocument> GetDocumentAsync()
    {
        await using var stream = await _client.GetStreamAsync(
            "/swagger/v1/swagger.json");

        return await JsonDocument.ParseAsync(stream);
    }

    /// <summary>
    /// Selects one controller operation from the generated OpenAPI paths.
    /// </summary>
    private static JsonElement GetOperation(
        JsonDocument document,
        string path,
        string method)
    {
        return document.RootElement
            .GetProperty("paths")
            .GetProperty(path)
            .GetProperty(method);
    }

    /// <summary>
    /// Confirms one response exposes only its intended media type.
    /// </summary>
    private static void AssertResponseContentType(
        JsonElement operation,
        string statusCode,
        string expectedContentType)
    {
        var content = operation
            .GetProperty("responses")
            .GetProperty(statusCode)
            .GetProperty("content")
            .EnumerateObject()
            .Single();

        Assert.Equal(expectedContentType, content.Name);
    }

    /// <summary>
    /// Confirms authentication failures declare no response body contract.
    /// </summary>
    private static void AssertEmptyResponse(
        JsonElement operation,
        string statusCode)
    {
        var response = operation
            .GetProperty("responses")
            .GetProperty(statusCode);

        Assert.False(response.TryGetProperty("content", out _));
    }

    /// <summary>
    /// Confirms a DTO has exactly the required, non-nullable properties given.
    /// </summary>
    private static void AssertRequiredProperties(
        JsonDocument document,
        string schemaName,
        params string[] expectedProperties)
    {
        var schema = document.RootElement
            .GetProperty("components")
            .GetProperty("schemas")
            .GetProperty(schemaName);
        var actualProperties = schema
            .GetProperty("required")
            .EnumerateArray()
            .Select(property => property.GetString())
            .Order()
            .ToArray();

        Assert.Equal(expectedProperties.Order(), actualProperties);

        foreach (var propertyName in expectedProperties)
        {
            var property = schema
                .GetProperty("properties")
                .GetProperty(propertyName);
            var isNullable = property.TryGetProperty(
                "nullable",
                out var nullable)
                && nullable.GetBoolean();

            Assert.False(isNullable);
        }
    }
}
