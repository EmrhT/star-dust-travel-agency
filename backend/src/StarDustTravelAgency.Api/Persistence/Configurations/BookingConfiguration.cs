using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarDustTravelAgency.Api.Domain.Entities;

// Maps Booking ownership and its required Journey relationship to PostgreSQL.
// ApplicationDbContext discovers this configuration with the API assembly.
namespace StarDustTravelAgency.Api.Persistence.Configurations;

/// <summary>
/// Maps Booking fields and prevents duplicate user-and-journey reservations.
/// Journey.Bookings supplies the inverse side of its restricted foreign key.
/// </summary>
public sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    // Applies Booking columns, its Journey foreign key, and owner uniqueness.
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("bookings");

        builder.HasKey(booking => booking.Id);

        builder.Property(booking => booking.Id)
            .HasColumnName("id");

        builder.Property(booking => booking.JourneyId)
            .HasColumnName("journey_id");

        builder.Property(booking => booking.UserSubject)
            .HasColumnName("user_subject")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(booking => booking.BookedAtUtc)
            .HasColumnName("booked_at_utc")
            .HasColumnType("timestamp with time zone");

        builder.HasOne(booking => booking.Journey)
            .WithMany(journey => journey.Bookings)
            .HasForeignKey(booking => booking.JourneyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(booking => new
        {
            booking.UserSubject,
            booking.JourneyId,
        })
            .IsUnique();
    }
}
