using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarDustTravelAgency.Api.Domain.Entities;

namespace StarDustTravelAgency.Api.Persistence.Configurations;

public sealed class LaunchStationConfiguration : IEntityTypeConfiguration<LaunchStation>
{
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
