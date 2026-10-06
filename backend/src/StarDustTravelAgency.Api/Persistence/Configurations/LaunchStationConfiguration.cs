using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarDustTravelAgency.Api.Domain.Entities;

// Maps LaunchStation properties and uniqueness rules to PostgreSQL.
// ApplicationDbContext discovers this class while building the EF Core model.
namespace StarDustTravelAgency.Api.Persistence.Configurations;

/// <summary>
/// Maps LaunchStation fields and its unique name to PostgreSQL.
/// ApplicationDbContext discovers it through OnModelCreating.
/// </summary>
public sealed class LaunchStationConfiguration
    : IEntityTypeConfiguration<LaunchStation>
{
    // Applies the station table, column, default, and unique-index mappings.
    public void Configure(EntityTypeBuilder<LaunchStation> builder)
    {
        builder.ToTable("launch_stations");

        builder.HasKey(station => station.Id);

        builder.Property(station => station.Id)
            .HasColumnName("id");

        builder.Property(station => station.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(station => station.Active)
            .HasColumnName("active")
            .HasDefaultValue(true);

        builder.HasIndex(station => station.Name)
            .IsUnique();
    }
}
