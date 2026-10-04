using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi;
using Npgsql;
using StarDustTravelAgency.Api.Health;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureHostOptions(options =>
{
    options.ShutdownTimeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Star Dust Travel Agency API",
        Version = "v1",
    });
});

var connectionString = builder.Configuration.GetConnectionString("Postgres");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "ConnectionStrings:Postgres must be provided through configuration.");
}

builder.Services.AddSingleton(_ => NpgsqlDataSource.Create(connectionString));
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

if (app.Configuration.GetValue("Swagger:Enabled", app.Environment.IsDevelopment()))
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Star Dust Travel Agency API v1");
        options.DocumentTitle = "Star Dust Travel Agency API";
    });
}

app.MapControllers();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains(HealthCheckTags.Live),
    ResponseWriter = HealthResponseWriter.WriteAsync,
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains(HealthCheckTags.Ready),
    ResponseWriter = HealthResponseWriter.WriteAsync,
});

app.Run();

public partial class Program
{
}
