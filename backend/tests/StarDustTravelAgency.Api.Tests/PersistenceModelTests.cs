using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using StarDustTravelAgency.Api.Domain.Entities;
using StarDustTravelAgency.Api.Persistence;
using TravelRoute = StarDustTravelAgency.Api.Domain.Entities.Route;

namespace StarDustTravelAgency.Api.Tests;

public sealed class PersistenceModelTests
{
    [Fact]
    public void Operational_core_model_builds_with_expected_tables_and_relationships()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Database=model_test;Username=test;Password=test")
            .Options;

        using var context = new ApplicationDbContext(options);

        Assert.Equal("destinations", context.Model.FindEntityType(typeof(Destination))?.GetTableName());
        Assert.Equal("journeys", context.Model.FindEntityType(typeof(Journey))?.GetTableName());
        Assert.Equal("launch_stations", context.Model.FindEntityType(typeof(LaunchStation))?.GetTableName());
        Assert.Equal("routes", context.Model.FindEntityType(typeof(TravelRoute))?.GetTableName());
        Assert.Equal("spaceships", context.Model.FindEntityType(typeof(Spaceship))?.GetTableName());
        Assert.Equal("weekly_schedules", context.Model.FindEntityType(typeof(WeeklySchedule))?.GetTableName());

        var weeklyScheduleForeignKey = context.Model
            .FindEntityType(typeof(Journey))?
            .FindProperty(nameof(Journey.WeeklyScheduleId))?
            .GetContainingForeignKeys()
            .Single();

        Assert.False(weeklyScheduleForeignKey?.IsRequired);
        Assert.Equal(DeleteBehavior.Restrict, weeklyScheduleForeignKey?.DeleteBehavior);
    }
}
