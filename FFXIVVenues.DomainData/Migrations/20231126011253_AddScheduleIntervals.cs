using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FFXIVVenues.DomainData.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduleIntervals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Commencing",
                schema: "Venues",
                table: "Schedules",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IntervalType",
                schema: "Venues",
                table: "Schedules",
                type: "integer",
                nullable: false,
                defaultValue: 0);
            
            migrationBuilder.AddColumn<int>(
                name: "IntervalArgument",
                schema: "Venues",
                table: "Schedules",
                type: "integer",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "From",
                schema: "Venues",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "IntervalType",
                schema: "Venues",
                table: "Schedules");
            
            migrationBuilder.DropColumn(
                name: "IntervalArgument",
                schema: "Venues",
                table: "Schedules");
        }
    }
}
