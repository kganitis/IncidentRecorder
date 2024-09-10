using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentRecorder.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintToPatientNIN : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NIN",
                table: "Patients",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_NIN",
                table: "Patients",
                column: "NIN",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Patients_NIN",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "NIN",
                table: "Patients");
        }
    }
}
