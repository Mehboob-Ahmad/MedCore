using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicHp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientMedicalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FamilyMedicalHistory",
                schema: "clinical",
                table: "PatientProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImmunizationHistory",
                schema: "clinical",
                table: "PatientProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LifestyleInformation",
                schema: "clinical",
                table: "PatientProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MedicalHistory",
                schema: "clinical",
                table: "PatientProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PatientHospitalizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    AdmissionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DischargeDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HospitalName = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientHospitalizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientHospitalizations_PatientProfiles_PatientProfileId",
                        column: x => x.PatientProfileId,
                        principalSchema: "clinical",
                        principalTable: "PatientProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientSurgeries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    SurgeryName = table.Column<string>(type: "text", nullable: false),
                    SurgeryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SurgeonName = table.Column<string>(type: "text", nullable: true),
                    HospitalName = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientSurgeries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientSurgeries_PatientProfiles_PatientProfileId",
                        column: x => x.PatientProfileId,
                        principalSchema: "clinical",
                        principalTable: "PatientProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientHospitalizations_PatientProfileId",
                table: "PatientHospitalizations",
                column: "PatientProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientSurgeries_PatientProfileId",
                table: "PatientSurgeries",
                column: "PatientProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientHospitalizations");

            migrationBuilder.DropTable(
                name: "PatientSurgeries");

            migrationBuilder.DropColumn(
                name: "FamilyMedicalHistory",
                schema: "clinical",
                table: "PatientProfiles");

            migrationBuilder.DropColumn(
                name: "ImmunizationHistory",
                schema: "clinical",
                table: "PatientProfiles");

            migrationBuilder.DropColumn(
                name: "LifestyleInformation",
                schema: "clinical",
                table: "PatientProfiles");

            migrationBuilder.DropColumn(
                name: "MedicalHistory",
                schema: "clinical",
                table: "PatientProfiles");
        }
    }
}
