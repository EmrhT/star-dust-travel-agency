using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarDustTravelAgency.Api.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingFormDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "contact_email",
                table: "bookings",
                type: "character varying(254)",
                maxLength: 254,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "number_of_travelers",
                table: "bookings",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "special_requests",
                table: "bookings",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE bookings
                SET contact_email = 'legacy-booking@example.invalid',
                    number_of_travelers = 1;
                """);

            migrationBuilder.AlterColumn<string>(
                name: "contact_email",
                table: "bookings",
                type: "character varying(254)",
                maxLength: 254,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(254)",
                oldMaxLength: 254,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "number_of_travelers",
                table: "bookings",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "ck_bookings_number_of_travelers",
                table: "bookings",
                sql: "number_of_travelers BETWEEN 1 AND 6");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_bookings_number_of_travelers",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "contact_email",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "number_of_travelers",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "special_requests",
                table: "bookings");
        }
    }
}
