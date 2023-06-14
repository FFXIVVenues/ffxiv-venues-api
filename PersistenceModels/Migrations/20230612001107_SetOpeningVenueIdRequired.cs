using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FFXIVVenues.Api.PersistenceModels.Migrations
{
    /// <inheritdoc />
    public partial class SetOpeningVenueIdRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "VenueId",
                schema: "Venues",
                table: "Openings",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "VenueId",
                schema: "Venues",
                table: "Openings",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
