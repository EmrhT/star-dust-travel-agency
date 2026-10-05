using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarDustTravelAgency.Api.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialOperationalCore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "destinations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    star_system = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    short_description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    gravity_relative_to_earth = table.Column<decimal>(type: "numeric(6,3)", precision: 6, scale: 3, nullable: false),
                    average_temperature_celsius = table.Column<decimal>(type: "numeric(7,2)", precision: 7, scale: 2, nullable: false),
                    travel_advisory = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_destinations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "launch_stations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_launch_stations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "spaceships",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    capacity = table.Column<int>(type: "integer", nullable: false),
                    year_built = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_spaceships", x => x.id);
                    table.CheckConstraint("ck_spaceships_capacity", "capacity > 0");
                    table.CheckConstraint("ck_spaceships_year_built", "year_built BETWEEN 2000 AND 9999");
                });

            migrationBuilder.CreateTable(
                name: "routes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    launch_station_id = table.Column<Guid>(type: "uuid", nullable: false),
                    destination_id = table.Column<Guid>(type: "uuid", nullable: false),
                    duration_minutes = table.Column<int>(type: "integer", nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_routes", x => x.id);
                    table.CheckConstraint("ck_routes_duration_minutes", "duration_minutes BETWEEN 1 AND 960");
                    table.ForeignKey(
                        name: "FK_routes_destinations_destination_id",
                        column: x => x.destination_id,
                        principalTable: "destinations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_routes_launch_stations_launch_station_id",
                        column: x => x.launch_station_id,
                        principalTable: "launch_stations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "weekly_schedules",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    route_id = table.Column<Guid>(type: "uuid", nullable: false),
                    direction = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    departure_day = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    departure_time = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    default_spaceship_id = table.Column<Guid>(type: "uuid", nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_weekly_schedules", x => x.id);
                    table.ForeignKey(
                        name: "FK_weekly_schedules_routes_route_id",
                        column: x => x.route_id,
                        principalTable: "routes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_weekly_schedules_spaceships_default_spaceship_id",
                        column: x => x.default_spaceship_id,
                        principalTable: "spaceships",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "journeys",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    route_id = table.Column<Guid>(type: "uuid", nullable: false),
                    weekly_schedule_id = table.Column<Guid>(type: "uuid", nullable: true),
                    direction = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    departure_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    arrival_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    spaceship_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_journeys", x => x.id);
                    table.CheckConstraint("ck_journeys_arrival_after_departure", "arrival_at_utc > departure_at_utc");
                    table.CheckConstraint("ck_journeys_maximum_duration", "arrival_at_utc <= departure_at_utc + INTERVAL '16 hours'");
                    table.ForeignKey(
                        name: "FK_journeys_routes_route_id",
                        column: x => x.route_id,
                        principalTable: "routes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_journeys_spaceships_spaceship_id",
                        column: x => x.spaceship_id,
                        principalTable: "spaceships",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_journeys_weekly_schedules_weekly_schedule_id",
                        column: x => x.weekly_schedule_id,
                        principalTable: "weekly_schedules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_destinations_name",
                table: "destinations",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_journeys_route_id",
                table: "journeys",
                column: "route_id");

            migrationBuilder.CreateIndex(
                name: "IX_journeys_spaceship_id_departure_at_utc_arrival_at_utc",
                table: "journeys",
                columns: new[] { "spaceship_id", "departure_at_utc", "arrival_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_journeys_status_departure_at_utc",
                table: "journeys",
                columns: new[] { "status", "departure_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_journeys_weekly_schedule_id",
                table: "journeys",
                column: "weekly_schedule_id");

            migrationBuilder.CreateIndex(
                name: "IX_launch_stations_name",
                table: "launch_stations",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_routes_destination_id",
                table: "routes",
                column: "destination_id");

            migrationBuilder.CreateIndex(
                name: "IX_routes_launch_station_id_destination_id",
                table: "routes",
                columns: new[] { "launch_station_id", "destination_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_spaceships_name",
                table: "spaceships",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_weekly_schedules_default_spaceship_id",
                table: "weekly_schedules",
                column: "default_spaceship_id");

            migrationBuilder.CreateIndex(
                name: "IX_weekly_schedules_route_id_direction_departure_day_departure~",
                table: "weekly_schedules",
                columns: new[] { "route_id", "direction", "departure_day", "departure_time" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "journeys");

            migrationBuilder.DropTable(
                name: "weekly_schedules");

            migrationBuilder.DropTable(
                name: "routes");

            migrationBuilder.DropTable(
                name: "spaceships");

            migrationBuilder.DropTable(
                name: "destinations");

            migrationBuilder.DropTable(
                name: "launch_stations");
        }
    }
}
