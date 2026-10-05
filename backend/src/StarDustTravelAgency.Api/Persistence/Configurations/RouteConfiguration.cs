using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarDustTravelAgency.Api.Domain.Entities;
using TravelRoute = StarDustTravelAgency.Api.Domain.Entities.Route;

namespace StarDustTravelAgency.Api.Persistence.Configurations;

public sealed class RouteConfiguration : IEntityTypeConfiguration<TravelRoute>
{
    public void Configure(EntityTypeBuilder<TravelRoute> builder)
    {
        builder.ToTable("routes", table =>
            table.HasCheckConstraint(
                "ck_routes_duration_minutes",
                "duration_minutes BETWEEN 1 AND 960"));

        builder.HasKey(route => route.Id);

        builder.Property(route => route.Id)
            .HasColumnName("id");

        builder.Property(route => route.LaunchStationId)
            .HasColumnName("launch_station_id");

        builder.Property(route => route.DestinationId)
            .HasColumnName("destination_id");

        builder.Property(route => route.DurationMinutes)
            .HasColumnName("duration_minutes");

        builder.Property(route => route.Active)
            .HasColumnName("active")
            .HasDefaultValue(true);

        builder.HasOne(route => route.LaunchStation)
            .WithMany(station => station.Routes)
            .HasForeignKey(route => route.LaunchStationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(route => route.Destination)
            .WithMany(destination => destination.Routes)
            .HasForeignKey(route => route.DestinationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(route => new { route.LaunchStationId, route.DestinationId })
            .IsUnique();
    }
}
