using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FFXIVVenues.DomainData.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNullability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Locations_LocationId",
                schema: "Venues",
                table: "Schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_VenueViews_Venues_VenueId",
                schema: "VenueMetrics",
                table: "VenueViews");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_LocationId",
                schema: "Venues",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "LocationId",
                schema: "Venues",
                table: "Schedules");

            migrationBuilder.AlterColumn<string>(
                name: "VenueId",
                schema: "VenueMetrics",
                table: "VenueViews",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "Venues",
                table: "Venues",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TimeZone",
                schema: "Venues",
                table: "Schedules",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Start",
                schema: "Venues",
                table: "Notices",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                schema: "Venues",
                table: "Notices",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "End",
                schema: "Venues",
                table: "Notices",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "World",
                schema: "Venues",
                table: "Locations",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "District",
                schema: "Venues",
                table: "Locations",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DataCenter",
                schema: "Venues",
                table: "Locations",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "Address",
                schema: "Venues",
                table: "Locations",
                columns: new[] { "DataCenter", "World", "District", "Ward", "Plot", "Apartment", "Room" });

            migrationBuilder.CreateIndex(
                name: "Override",
                schema: "Venues",
                table: "Locations",
                column: "Override");

            migrationBuilder.AddForeignKey(
                name: "FK_VenueViews_Venues_VenueId",
                schema: "VenueMetrics",
                table: "VenueViews",
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
                name: "FK_VenueViews_Venues_VenueId",
                schema: "VenueMetrics",
                table: "VenueViews");

            migrationBuilder.DropIndex(
                name: "Address",
                schema: "Venues",
                table: "Locations");

            migrationBuilder.DropIndex(
                name: "Override",
                schema: "Venues",
                table: "Locations");

            migrationBuilder.AlterColumn<string>(
                name: "VenueId",
                schema: "VenueMetrics",
                table: "VenueViews",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "Venues",
                table: "Venues",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "TimeZone",
                schema: "Venues",
                table: "Schedules",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "LocationId",
                schema: "Venues",
                table: "Schedules",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Start",
                schema: "Venues",
                table: "Notices",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                schema: "Venues",
                table: "Notices",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "End",
                schema: "Venues",
                table: "Notices",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "World",
                schema: "Venues",
                table: "Locations",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "District",
                schema: "Venues",
                table: "Locations",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "DataCenter",
                schema: "Venues",
                table: "Locations",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_LocationId",
                schema: "Venues",
                table: "Schedules",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Locations_LocationId",
                schema: "Venues",
                table: "Schedules",
                column: "LocationId",
                principalSchema: "Venues",
                principalTable: "Locations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VenueViews_Venues_VenueId",
                schema: "VenueMetrics",
                table: "VenueViews",
                column: "VenueId",
                principalSchema: "Venues",
                principalTable: "Venues",
                principalColumn: "Id");
        }
    }
}
