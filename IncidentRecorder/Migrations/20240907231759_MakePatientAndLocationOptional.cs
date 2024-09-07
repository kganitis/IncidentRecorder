using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentRecorder.Migrations
{
    /// <inheritdoc />
    public partial class MakePatientAndLocationOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_Locations_LocationId",
                table: "Incidents");

            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_Patients_PatientId",
                table: "Incidents");

            migrationBuilder.AlterColumn<int>(
                name: "PatientId",
                table: "Incidents",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "LocationId",
                table: "Incidents",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_Locations_LocationId",
                table: "Incidents",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_Patients_PatientId",
                table: "Incidents",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_Locations_LocationId",
                table: "Incidents");

            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_Patients_PatientId",
                table: "Incidents");

            migrationBuilder.AlterColumn<int>(
                name: "PatientId",
                table: "Incidents",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LocationId",
                table: "Incidents",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_Locations_LocationId",
                table: "Incidents",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_Patients_PatientId",
                table: "Incidents",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
