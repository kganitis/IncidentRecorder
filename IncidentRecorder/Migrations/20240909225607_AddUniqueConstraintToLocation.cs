using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentRecorder.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintToLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Locations_City_Country",
                table: "Locations",
                columns: new[] { "City", "Country" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Locations_City_Country",
                table: "Locations");
        }
    }
}
