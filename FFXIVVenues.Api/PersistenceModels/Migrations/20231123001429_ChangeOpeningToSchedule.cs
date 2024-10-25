using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FFXIVVenues.Api.PersistenceModels.Migrations
{
    /// <inheritdoc />
    public partial class ChangeOpeningToSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Openings_Locations_LocationId",
                schema: "Venues",
                table: "Openings");

            migrationBuilder.DropForeignKey(
                name: "FK_Openings_Venues_VenueId",
                schema: "Venues",
                table: "Openings");

            migrationBuilder.DropForeignKey(
                name: "FK_OpenOverrides_Venues_VenueId",
                schema: "Venues",
                table: "OpenOverrides");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OpenOverrides",
                schema: "Venues",
                table: "OpenOverrides");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Openings",
                schema: "Venues",
                table: "Openings");

            migrationBuilder.RenameTable(
                name: "OpenOverrides",
                schema: "Venues",
                newName: "ScheduleOverrides",
                newSchema: "Venues");

            migrationBuilder.RenameTable(
                name: "Openings",
                schema: "Venues",
                newName: "Schedules",
                newSchema: "Venues");

            migrationBuilder.RenameIndex(
                name: "IX_Openings_LocationId",
                schema: "Venues",
                table: "Schedules",
                newName: "IX_Schedules_LocationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScheduleOverrides",
                schema: "Venues",
                table: "ScheduleOverrides",
                columns: new[] { "VenueId", "Start" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Schedules",
                schema: "Venues",
                table: "Schedules",
                columns: new[] { "VenueId", "Day", "StartHour", "StartMinute" });

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleOverrides_Venues_VenueId",
                schema: "Venues",
                table: "ScheduleOverrides",
                column: "VenueId",
                principalSchema: "Venues",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Locations_LocationId",
                schema: "Venues",
                table: "Schedules",
                column: "LocationId",
                principalSchema: "Venues",
                principalTable: "Locations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Venues_VenueId",
                schema: "Venues",
                table: "Schedules",
                column: "VenueId",
                principalSchema: "Venues",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleOverrides_Venues_VenueId",
                schema: "Venues",
                table: "ScheduleOverrides");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Locations_LocationId",
                schema: "Venues",
                table: "Schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Venues_VenueId",
                schema: "Venues",
                table: "Schedules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Schedules",
                schema: "Venues",
                table: "Schedules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScheduleOverrides",
                schema: "Venues",
                table: "ScheduleOverrides");

            migrationBuilder.RenameTable(
                name: "Schedules",
                schema: "Venues",
                newName: "Openings",
                newSchema: "Venues");

            migrationBuilder.RenameTable(
                name: "ScheduleOverrides",
                schema: "Venues",
                newName: "OpenOverrides",
                newSchema: "Venues");

            migrationBuilder.RenameIndex(
                name: "IX_Schedules_LocationId",
                schema: "Venues",
                table: "Openings",
                newName: "IX_Openings_LocationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Openings",
                schema: "Venues",
                table: "Openings",
                columns: new[] { "VenueId", "Day", "StartHour", "StartMinute" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_OpenOverrides",
                schema: "Venues",
                table: "OpenOverrides",
                columns: new[] { "VenueId", "Start" });

            migrationBuilder.AddForeignKey(
                name: "FK_Openings_Locations_LocationId",
                schema: "Venues",
                table: "Openings",
                column: "LocationId",
                principalSchema: "Venues",
                principalTable: "Locations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Openings_Venues_VenueId",
                schema: "Venues",
                table: "Openings",
                column: "VenueId",
                principalSchema: "Venues",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OpenOverrides_Venues_VenueId",
                schema: "Venues",
                table: "OpenOverrides",
                column: "VenueId",
                principalSchema: "Venues",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
