using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarDustTravelAgency.Api.Domain.Entities;

namespace StarDustTravelAgency.Api.Persistence.Configurations;

public sealed class WeeklyScheduleConfiguration : IEntityTypeConfiguration<WeeklySchedule>
{
    public void Configure(EntityTypeBuilder<WeeklySchedule> builder)
    {
        builder.ToTable("weekly_schedules");

        builder.HasKey(schedule => schedule.Id);

        builder.Property(schedule => schedule.Id)
            .HasColumnName("id");

        builder.Property(schedule => schedule.RouteId)
            .HasColumnName("route_id");

        builder.Property(schedule => schedule.Direction)
            .HasColumnName("direction")
            .HasConversion<string>()
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(schedule => schedule.DepartureDay)
            .HasColumnName("departure_day")
            .HasConversion<string>()
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(schedule => schedule.DepartureTime)
            .HasColumnName("departure_time")
            .HasColumnType("time without time zone");

        builder.Property(schedule => schedule.DefaultSpaceshipId)
            .HasColumnName("default_spaceship_id");

        builder.Property(schedule => schedule.Active)
            .HasColumnName("active")
            .HasDefaultValue(true);

        builder.HasOne(schedule => schedule.Route)
            .WithMany(route => route.WeeklySchedules)
            .HasForeignKey(schedule => schedule.RouteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(schedule => schedule.DefaultSpaceship)
            .WithMany(spaceship => spaceship.WeeklySchedules)
            .HasForeignKey(schedule => schedule.DefaultSpaceshipId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(schedule => new
        {
            schedule.RouteId,
            schedule.Direction,
            schedule.DepartureDay,
            schedule.DepartureTime,
        }).IsUnique();
    }
}
