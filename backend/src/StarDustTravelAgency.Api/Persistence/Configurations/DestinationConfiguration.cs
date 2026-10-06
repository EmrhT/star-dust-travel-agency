using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarDustTravelAgency.Api.Domain.Entities;

// Maps Destination properties and validation rules to PostgreSQL columns.
// ApplicationDbContext discovers this class while building the EF Core model.
namespace StarDustTravelAgency.Api.Persistence.Configurations;

/// <summary>
/// Maps Destination fields, enum conversion, and its unique name.
/// ApplicationDbContext discovers it through OnModelCreating.
/// </summary>
public sealed class DestinationConfiguration
    : IEntityTypeConfiguration<Destination>
{
    // Applies table, column, precision, default, and uniqueness mappings.
    public void Configure(EntityTypeBuilder<Destination> builder)
    {
        builder.ToTable("destinations");

        builder.HasKey(destination => destination.Id);

        builder.Property(destination => destination.Id)
            .HasColumnName("id");

        builder.Property(destination => destination.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(destination => destination.Type)
            .HasColumnName("type")
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(destination => destination.StarSystem)
            .HasColumnName("star_system")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(destination => destination.ShortDescription)
            .HasColumnName("short_description")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(destination => destination.GravityRelativeToEarth)
            .HasColumnName("gravity_relative_to_earth")
            .HasPrecision(6, 3);

        builder.Property(destination => destination.AverageTemperatureCelsius)
            .HasColumnName("average_temperature_celsius")
            .HasPrecision(7, 2);

        builder.Property(destination => destination.TravelAdvisory)
            .HasColumnName("travel_advisory")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(destination => destination.Active)
            .HasColumnName("active")
            .HasDefaultValue(true);

        builder.HasIndex(destination => destination.Name)
            .IsUnique();
    }
}
