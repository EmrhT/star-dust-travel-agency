using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarDustTravelAgency.Api.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueJourneyOccurrence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_journeys_weekly_schedule_id",
                table: "journeys");

            migrationBuilder.CreateIndex(
                name: "IX_journeys_weekly_schedule_id_departure_at_utc",
                table: "journeys",
                columns: new[] { "weekly_schedule_id", "departure_at_utc" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_journeys_weekly_schedule_id_departure_at_utc",
                table: "journeys");

            migrationBuilder.CreateIndex(
                name: "IX_journeys_weekly_schedule_id",
                table: "journeys",
                column: "weekly_schedule_id");
        }
    }
}
