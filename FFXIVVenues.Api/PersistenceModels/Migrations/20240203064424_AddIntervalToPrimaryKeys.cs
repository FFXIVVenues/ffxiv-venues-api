using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FFXIVVenues.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddIntervalToPrimaryKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Schedules",
                schema: "Venues",
                table: "Schedules");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Commencing",
                schema: "Venues",
                table: "Schedules",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Schedules",
                schema: "Venues",
                table: "Schedules",
                columns: new[] { "VenueId", "Day", "StartHour", "StartMinute", "IntervalType", "IntervalArgument" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Schedules",
                schema: "Venues",
                table: "Schedules");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Commencing",
                schema: "Venues",
                table: "Schedules",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Schedules",
                schema: "Venues",
                table: "Schedules",
                columns: new[] { "VenueId", "Day", "StartHour", "StartMinute", "Commencing", "IntervalType", "IntervalArgument" });
        }
    }
}
