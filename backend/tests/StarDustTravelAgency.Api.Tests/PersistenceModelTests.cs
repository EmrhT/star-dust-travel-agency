using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using StarDustTravelAgency.Api.Domain.Entities;
using StarDustTravelAgency.Api.Persistence;
using TravelRoute = StarDustTravelAgency.Api.Domain.Entities.Route;

// Validates the EF Core metadata assembled by ApplicationDbContext.
// The test checks mappings without connecting to a running PostgreSQL server.
namespace StarDustTravelAgency.Api.Tests;

/// <summary>
/// Builds ApplicationDbContext to inspect all entity configuration classes.
/// It verifies table names and Journey's WeeklySchedule relation.
/// </summary>
public sealed class PersistenceModelTests
{
    /// <summary>
    /// Verifies ApplicationDbContext tables and Journey's optional relation.
    /// </summary>
    [Fact]
    public void Operational_model_has_expected_tables_and_relationships()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(
                "Host=localhost;Database=model_test;" +
                "Username=test;Password=test")
            .Options;

        using var context = new ApplicationDbContext(options);

        Assert.Equal(
            "destinations",
            context.Model.FindEntityType(typeof(Destination))?.GetTableName());
        Assert.Equal(
            "journeys",
            context.Model.FindEntityType(typeof(Journey))?.GetTableName());
        Assert.Equal(
            "launch_stations",
            context.Model
                .FindEntityType(typeof(LaunchStation))?
                .GetTableName());
        Assert.Equal(
            "routes",
            context.Model.FindEntityType(typeof(TravelRoute))?.GetTableName());
        Assert.Equal(
            "spaceships",
            context.Model.FindEntityType(typeof(Spaceship))?.GetTableName());
        Assert.Equal(
            "weekly_schedules",
            context.Model
                .FindEntityType(typeof(WeeklySchedule))?
                .GetTableName());

        var weeklyScheduleForeignKey = context.Model
            .FindEntityType(typeof(Journey))?
            .FindProperty(nameof(Journey.WeeklyScheduleId))?
            .GetContainingForeignKeys()
            .Single();

        Assert.False(weeklyScheduleForeignKey?.IsRequired);
        Assert.Equal(
            DeleteBehavior.Restrict,
            weeklyScheduleForeignKey?.DeleteBehavior);
    }
}
