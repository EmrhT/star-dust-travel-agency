using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarDustTravelAgency.Api.Domain.Entities;

// Maps Booking ownership, submitted form values, and its Journey relationship.
// ApplicationDbContext discovers this configuration with the API assembly.
namespace StarDustTravelAgency.Api.Persistence.Configurations;

/// <summary>
/// Maps Booking form constraints and prevents duplicate Journey reservations.
/// Journey.Bookings supplies the inverse side of its restricted foreign key.
/// </summary>
public sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    // Applies Booking form columns, its Journey relation, and owner uniqueness.
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable(
            "bookings",
            table => table.HasCheckConstraint(
                "ck_bookings_number_of_travelers",
                "number_of_travelers BETWEEN 1 AND 6"));

        builder.HasKey(booking => booking.Id);

        builder.Property(booking => booking.Id)
            .HasColumnName("id");

        builder.Property(booking => booking.JourneyId)
            .HasColumnName("journey_id");

        builder.Property(booking => booking.UserSubject)
            .HasColumnName("user_subject")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(booking => booking.NumberOfTravelers)
            .HasColumnName("number_of_travelers")
            .IsRequired();

        builder.Property(booking => booking.ContactEmail)
            .HasColumnName("contact_email")
            .HasMaxLength(254)
            .IsRequired();

        builder.Property(booking => booking.SpecialRequests)
            .HasColumnName("special_requests")
            .HasMaxLength(500);

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
