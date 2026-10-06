using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarDustTravelAgency.Api.Domain.Entities;

// Maps Journey fields, constraints, and relationships to PostgreSQL.
// ApplicationDbContext discovers this class while building the EF Core model.
namespace StarDustTravelAgency.Api.Persistence.Configurations;

/// <summary>
/// Maps Journey constraints and links to Route, WeeklySchedule, and Spaceship.
/// ApplicationDbContext discovers it through OnModelCreating.
/// </summary>
public sealed class JourneyConfiguration : IEntityTypeConfiguration<Journey>
{
    // Applies table rules, foreign keys, conversions, and query indexes.
    public void Configure(EntityTypeBuilder<Journey> builder)
    {
        builder.ToTable("journeys", table =>
        {
            table.HasCheckConstraint(
                "ck_journeys_arrival_after_departure",
                "arrival_at_utc > departure_at_utc");
            table.HasCheckConstraint(
                "ck_journeys_maximum_duration",
                "arrival_at_utc <= departure_at_utc + INTERVAL '16 hours'");
        });

        builder.HasKey(journey => journey.Id);

        builder.Property(journey => journey.Id)
            .HasColumnName("id");

        builder.Property(journey => journey.RouteId)
            .HasColumnName("route_id");

        builder.Property(journey => journey.WeeklyScheduleId)
            .HasColumnName("weekly_schedule_id");

        builder.Property(journey => journey.Direction)
            .HasColumnName("direction")
            .HasConversion<string>()
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(journey => journey.DepartureAtUtc)
            .HasColumnName("departure_at_utc")
            .HasColumnType("timestamp with time zone");

        builder.Property(journey => journey.ArrivalAtUtc)
            .HasColumnName("arrival_at_utc")
            .HasColumnType("timestamp with time zone");

        builder.Property(journey => journey.SpaceshipId)
            .HasColumnName("spaceship_id");

        builder.Property(journey => journey.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.HasOne(journey => journey.Route)
            .WithMany(route => route.Journeys)
            .HasForeignKey(journey => journey.RouteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(journey => journey.WeeklySchedule)
            .WithMany(schedule => schedule.Journeys)
            .HasForeignKey(journey => journey.WeeklyScheduleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(journey => journey.Spaceship)
            .WithMany(spaceship => spaceship.Journeys)
            .HasForeignKey(journey => journey.SpaceshipId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(journey => new
        {
            journey.SpaceshipId,
            journey.DepartureAtUtc,
            journey.ArrivalAtUtc,
        });

        builder.HasIndex(journey => new
        {
            journey.Status,
            journey.DepartureAtUtc,
        });

        builder.HasIndex(journey => new
        {
            journey.WeeklyScheduleId,
            journey.DepartureAtUtc,
        }).IsUnique();
    }
}
