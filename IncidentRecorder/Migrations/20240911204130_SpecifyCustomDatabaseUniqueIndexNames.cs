using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentRecorder.Migrations
{
    /// <inheritdoc />
    public partial class SpecifyCustomDatabaseUniqueIndexNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_Symptoms_Name",
                table: "Symptoms",
                newName: "IX_Unique_Symptom_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Patients_NIN",
                table: "Patients",
                newName: "IX_Unique_Patient_NIN");

            migrationBuilder.RenameIndex(
                name: "IX_Locations_City_Country",
                table: "Locations",
                newName: "IX_Unique_Location_City_Country");

            migrationBuilder.RenameIndex(
                name: "IX_Diseases_Name",
                table: "Diseases",
                newName: "IX_Unique_Disease_Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_Unique_Symptom_Name",
                table: "Symptoms",
                newName: "IX_Symptoms_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Unique_Patient_NIN",
                table: "Patients",
                newName: "IX_Patients_NIN");

            migrationBuilder.RenameIndex(
                name: "IX_Unique_Location_City_Country",
                table: "Locations",
                newName: "IX_Locations_City_Country");

            migrationBuilder.RenameIndex(
                name: "IX_Unique_Disease_Name",
                table: "Diseases",
                newName: "IX_Diseases_Name");
        }
    }
}
