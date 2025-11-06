using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FFXIVVenues.DomainData.Migrations;

/// <inheritdoc />
public partial class Initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "Venues");

        migrationBuilder.EnsureSchema(
            name: "VenueMetrics");

        migrationBuilder.CreateTable(
            name: "Locations",
            schema: "Venues",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                DataCenter = table.Column<string>(type: "text", nullable: true),
                World = table.Column<string>(type: "text", nullable: true),
                District = table.Column<string>(type: "text", nullable: true),
                Ward = table.Column<int>(type: "integer", nullable: false),
                Plot = table.Column<int>(type: "integer", nullable: false),
                Apartment = table.Column<int>(type: "integer", nullable: false),
                Room = table.Column<int>(type: "integer", nullable: false),
                Subdivision = table.Column<bool>(type: "boolean", nullable: false),
                Override = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Locations", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Venues",
            schema: "Venues",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                Name = table.Column<string>(type: "text", nullable: true),
                Banner = table.Column<string>(type: "text", nullable: true),
                Added = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                Description = table.Column<List<string>>(type: "text[]", nullable: true),
                LocationId = table.Column<string>(type: "text", nullable: true),
                Website = table.Column<string>(type: "text", nullable: true),
                Discord = table.Column<string>(type: "text", nullable: true),
                Hiring = table.Column<bool>(type: "boolean", nullable: false),
                Sfw = table.Column<bool>(type: "boolean", nullable: false),
                Managers = table.Column<List<string>>(type: "text[]", nullable: true),
                Tags = table.Column<List<string>>(type: "text[]", nullable: true),
                MareCode = table.Column<string>(type: "text", nullable: true),
                MarePassword = table.Column<string>(type: "text", nullable: true),
                Approved = table.Column<bool>(type: "boolean", nullable: false),
                ScopeKey = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Venues", x => x.Id);
                table.ForeignKey(
                    name: "FK_Venues_Locations_LocationId",
                    column: x => x.LocationId,
                    principalSchema: "Venues",
                    principalTable: "Locations",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "Notices",
            schema: "Venues",
            columns: table => new
            {
                VenueId = table.Column<string>(type: "text", nullable: false),
                Id = table.Column<string>(type: "text", nullable: false),
                Start = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                End = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                Type = table.Column<int>(type: "integer", nullable: false),
                Message = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Notices", x => new { x.VenueId, x.Id });
                table.ForeignKey(
                    name: "FK_Notices_Venues_VenueId",
                    column: x => x.VenueId,
                    principalSchema: "Venues",
                    principalTable: "Venues",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Openings",
            schema: "Venues",
            columns: table => new
            {
                VenueId = table.Column<string>(type: "text", nullable: false),
                Day = table.Column<int>(type: "integer", nullable: false),
                StartHour = table.Column<int>(type: "integer", nullable: false),
                StartMinute = table.Column<int>(type: "integer", nullable: false),
                EndHour = table.Column<int>(type: "integer", nullable: true),
                EndMinute = table.Column<int>(type: "integer", nullable: true),
                TimeZone = table.Column<string>(type: "text", nullable: true),
                LocationId = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Openings", x => new { x.VenueId, x.Day, x.StartHour, x.StartMinute });
                table.ForeignKey(
                    name: "FK_Openings_Locations_LocationId",
                    column: x => x.LocationId,
                    principalSchema: "Venues",
                    principalTable: "Locations",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_Openings_Venues_VenueId",
                    column: x => x.VenueId,
                    principalSchema: "Venues",
                    principalTable: "Venues",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "OpenOverrides",
            schema: "Venues",
            columns: table => new
            {
                VenueId = table.Column<string>(type: "text", nullable: false),
                Start = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                Open = table.Column<bool>(type: "boolean", nullable: false),
                End = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OpenOverrides", x => new { x.VenueId, x.Start });
                table.ForeignKey(
                    name: "FK_OpenOverrides_Venues_VenueId",
                    column: x => x.VenueId,
                    principalSchema: "Venues",
                    principalTable: "Venues",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "VenueViews",
            schema: "VenueMetrics",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                At = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                VenueId = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_VenueViews", x => x.Id);
                table.ForeignKey(
                    name: "FK_VenueViews_Venues_VenueId",
                    column: x => x.VenueId,
                    principalSchema: "Venues",
                    principalTable: "Venues",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateIndex(
            name: "IX_Openings_LocationId",
            schema: "Venues",
            table: "Openings",
            column: "LocationId");

        migrationBuilder.CreateIndex(
            name: "IX_Venues_LocationId",
            schema: "Venues",
            table: "Venues",
            column: "LocationId");

        migrationBuilder.CreateIndex(
            name: "IX_VenueViews_VenueId",
            schema: "VenueMetrics",
            table: "VenueViews",
            column: "VenueId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Notices",
            schema: "Venues");

        migrationBuilder.DropTable(
            name: "Openings",
            schema: "Venues");

        migrationBuilder.DropTable(
            name: "OpenOverrides",
            schema: "Venues");

        migrationBuilder.DropTable(
            name: "VenueViews",
            schema: "VenueMetrics");

        migrationBuilder.DropTable(
            name: "Venues",
            schema: "Venues");

        migrationBuilder.DropTable(
            name: "Locations",
            schema: "Venues");
    }
}