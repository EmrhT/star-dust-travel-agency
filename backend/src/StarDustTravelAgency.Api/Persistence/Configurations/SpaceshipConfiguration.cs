using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarDustTravelAgency.Api.Domain.Entities;

namespace StarDustTravelAgency.Api.Persistence.Configurations;

public sealed class SpaceshipConfiguration : IEntityTypeConfiguration<Spaceship>
{
    public void Configure(EntityTypeBuilder<Spaceship> builder)
    {
        builder.ToTable("spaceships", table =>
        {
            table.HasCheckConstraint(
                "ck_spaceships_capacity",
                "capacity > 0");
            table.HasCheckConstraint(
                "ck_spaceships_year_built",
                "year_built BETWEEN 2000 AND 9999");
        });

        builder.HasKey(spaceship => spaceship.Id);

        builder.Property(spaceship => spaceship.Id)
            .HasColumnName("id");

        builder.Property(spaceship => spaceship.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(spaceship => spaceship.Model)
            .HasColumnName("model")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(spaceship => spaceship.Capacity)
            .HasColumnName("capacity");

        builder.Property(spaceship => spaceship.YearBuilt)
            .HasColumnName("year_built");

        builder.Property(spaceship => spaceship.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.HasIndex(spaceship => spaceship.Name)
            .IsUnique();
    }
}
