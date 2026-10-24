using System.Security.Claims;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Npgsql;
using StarDustTravelAgency.Api.Health;
using StarDustTravelAgency.Api.Persistence;
using StarDustTravelAgency.Api.Persistence.Seeding;
using StarDustTravelAgency.Api.Security;
using StarDustTravelAgency.Api.Services.Journeys;

// Composes authentication, controllers, persistence, and health checks.
// It joins Keycloak token validation, authorization, EF Core, and PostgreSQL.
var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureHostOptions(options =>
{
    options.ShutdownTimeout = TimeSpan.FromSeconds(30);
});

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()));
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<JourneyGenerationService>();
builder.Services.AddScoped<JourneyQueryService>();

var authority = builder.Configuration["Authentication:Authority"];
if (string.IsNullOrWhiteSpace(authority))
{
    throw new InvalidOperationException(
        "Authentication:Authority must be provided through configuration.");
}

var audience = builder.Configuration["Authentication:Audience"];
if (string.IsNullOrWhiteSpace(audience))
{
    throw new InvalidOperationException(
        "Authentication:Audience must be provided through configuration.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = authority;
        options.Audience = audience;
        options.RequireHttpsMetadata = true;
        options.MapInboundClaims = false;
        options.TokenValidationParameters.NameClaimType =
            "preferred_username";
        options.TokenValidationParameters.RoleClaimType =
            ClaimTypes.Role;
    });
builder.Services.AddTransient<IClaimsTransformation>(_ =>
    new KeycloakRoleClaimsTransformation(audience));
builder.Services
    .AddAuthorizationBuilder()
    .SetFallbackPolicy(
        new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build())
    .AddPolicy(
        ApplicationAuthorization.JourneyOperatorPolicy,
        policy => policy.RequireRole(
            ApplicationAuthorization.JourneyOperatorRole));
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Star Dust Travel Agency API",
        Version = "v1",
    });
    options.IncludeXmlComments(
        typeof(Program).Assembly,
        includeControllerXmlComments: true);
    options.SupportNonNullableReferenceTypes();
    options.NonNullableReferenceTypesAsRequired();
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description =
            "Keycloak access token issued for the star-dust-api audience.",
    });
    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = [],
        });

    // Gives NSwag stable controller_action names for generated client methods.
    options.CustomOperationIds(apiDescription =>
    {
        var routeValues = apiDescription.ActionDescriptor.RouteValues;
        var controller = routeValues["controller"];
        var action = routeValues["action"];

        return $"{controller}_{action}";
    });
});

var connectionString = builder.Configuration.GetConnectionString("Postgres");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "ConnectionStrings:Postgres must be provided through configuration.");
}

builder.Services.AddSingleton(_ => NpgsqlDataSource.Create(connectionString));
builder.Services.AddDbContext<ApplicationDbContext>(
    (serviceProvider, options) =>
        options
            .UseNpgsql(
                serviceProvider.GetRequiredService<NpgsqlDataSource>())
            .UseSeeding(
                (context, _) => ReferenceDataSeeder.Seed(context))
            .UseAsyncSeeding((context, _, cancellationToken) =>
                ReferenceDataSeeder.SeedAsync(context, cancellationToken)));
builder.Services
    .AddHealthChecks()
    .AddCheck<ProcessLivenessHealthCheck>(
        "process",
        tags: [HealthCheckTags.Live])
    .AddCheck<PostgresReadinessHealthCheck>(
        "postgresql",
        tags: [HealthCheckTags.Ready]);

var app = builder.Build();

app.Logger.LogInformation(
    "Starting {ApplicationName} in {EnvironmentName}",
    "Star Dust Travel Agency API",
    app.Environment.EnvironmentName);

app.UseExceptionHandler();

if (app.Configuration.GetValue(
    "Swagger:Enabled",
    app.Environment.IsDevelopment()))
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "Star Dust Travel Agency API v1");
        options.DocumentTitle = "Star Dust Travel Agency API";
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = registration =>
        registration.Tags.Contains(HealthCheckTags.Live),
    ResponseWriter = HealthResponseWriter.WriteAsync,
}).AllowAnonymous();

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = registration =>
        registration.Tags.Contains(HealthCheckTags.Ready),
    ResponseWriter = HealthResponseWriter.WriteAsync,
}).AllowAnonymous();

app.Run();

/// <summary>
/// Exposes the top-level host to UnavailablePostgresApiFactory.
/// WebApplicationFactory uses it to boot API integration tests.
/// </summary>
public partial class Program
{
}
