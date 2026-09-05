using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicHp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePatientAndDoctorProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasNoDocuments",
                table: "PatientSurgeries",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "RegistrationNumber",
                schema: "clinical",
                table: "DoctorProfiles",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Specialization",
                schema: "clinical",
                table: "DoctorProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PatientMedicalReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportName = table.Column<string>(type: "text", nullable: false),
                    ReportType = table.Column<string>(type: "text", nullable: false),
                    ReportDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    FileId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientMedicalReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientMedicalReports_Files_FileId",
                        column: x => x.FileId,
                        principalSchema: "core",
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatientMedicalReports_PatientProfiles_PatientProfileId",
                        column: x => x.PatientProfileId,
                        principalSchema: "clinical",
                        principalTable: "PatientProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientSurgeryDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientSurgeryId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientSurgeryDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientSurgeryDocuments_Files_FileId",
                        column: x => x.FileId,
                        principalSchema: "core",
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatientSurgeryDocuments_PatientSurgeries_PatientSurgeryId",
                        column: x => x.PatientSurgeryId,
                        principalTable: "PatientSurgeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientMedicalReports_FileId",
                table: "PatientMedicalReports",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientMedicalReports_PatientProfileId",
                table: "PatientMedicalReports",
                column: "PatientProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientSurgeryDocuments_FileId",
                table: "PatientSurgeryDocuments",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientSurgeryDocuments_PatientSurgeryId",
                table: "PatientSurgeryDocuments",
                column: "PatientSurgeryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientMedicalReports");

            migrationBuilder.DropTable(
                name: "PatientSurgeryDocuments");

            migrationBuilder.DropColumn(
                name: "HasNoDocuments",
                table: "PatientSurgeries");

            migrationBuilder.DropColumn(
                name: "Specialization",
                schema: "clinical",
                table: "DoctorProfiles");

            migrationBuilder.AlterColumn<string>(
                name: "RegistrationNumber",
                schema: "clinical",
                table: "DoctorProfiles",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
