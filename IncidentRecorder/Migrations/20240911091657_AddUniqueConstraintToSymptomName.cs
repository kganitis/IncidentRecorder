using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentRecorder.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintToSymptomName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Symptoms_Name",
                table: "Symptoms",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Symptoms_Name",
                table: "Symptoms");
        }
    }
}
