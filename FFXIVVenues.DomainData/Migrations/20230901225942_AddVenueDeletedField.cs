using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FFXIVVenues.DomainData.Migrations;

/// <inheritdoc />
public partial class AddVenueDeletedField : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "Deleted",
            schema: "Venues",
            table: "Venues",
            type: "timestamp with time zone",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Deleted",
            schema: "Venues",
            table: "Venues");
    }
}