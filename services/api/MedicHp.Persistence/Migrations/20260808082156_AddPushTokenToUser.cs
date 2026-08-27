using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicHp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPushTokenToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Gender",
                schema: "core",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PushToken",
                schema: "core",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Quantity",
                schema: "clinical",
                table: "PrescriptionItems",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Route",
                schema: "clinical",
                table: "PrescriptionItems",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Strength",
                schema: "clinical",
                table: "PrescriptionItems",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Timing",
                schema: "clinical",
                table: "PrescriptionItems",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClinicName",
                schema: "clinical",
                table: "DoctorProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Languages",
                schema: "clinical",
                table: "DoctorProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FollowUpDate",
                schema: "clinical",
                table: "Consultations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FollowUpInstructions",
                schema: "clinical",
                table: "Consultations",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FollowUpUrgency",
                schema: "clinical",
                table: "Consultations",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PatientNotes",
                schema: "clinical",
                table: "Consultations",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrivateNotes",
                schema: "clinical",
                table: "Consultations",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VisitType",
                schema: "clinical",
                table: "Consultations",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "clinical",
                table: "Appointments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "CancellationReason",
                schema: "clinical",
                table: "Appointments",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BookingNote",
                schema: "clinical",
                table: "Appointments",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DoctorNotes",
                schema: "clinical",
                table: "Appointments",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                schema: "clinical",
                table: "Appointments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SuggestedNewTime",
                schema: "clinical",
                table: "Appointments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ConsultationTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Diagnosis = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ClinicalNotes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    TreatmentPlan = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    FollowUpInstructions = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultationTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsultationTemplates_DoctorProfiles_DoctorId",
                        column: x => x.DoctorId,
                        principalSchema: "clinical",
                        principalTable: "DoctorProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Diseases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diseases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DoctorFavoriteMedicines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    MedicationName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AddedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorFavoriteMedicines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoctorFavoriteMedicines_DoctorProfiles_DoctorId",
                        column: x => x.DoctorId,
                        principalSchema: "clinical",
                        principalTable: "DoctorProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatientFavoriteDoctors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientFavoriteDoctors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientFavoriteDoctors_DoctorProfiles_DoctorId",
                        column: x => x.DoctorId,
                        principalSchema: "clinical",
                        principalTable: "DoctorProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientFavoriteDoctors_PatientProfiles_PatientId",
                        column: x => x.PatientId,
                        principalSchema: "clinical",
                        principalTable: "PatientProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrescriptionTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrescriptionTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrescriptionTemplates_DoctorProfiles_DoctorId",
                        column: x => x.DoctorId,
                        principalSchema: "clinical",
                        principalTable: "DoctorProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DiseaseSpecializations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DiseaseId = table.Column<Guid>(type: "uuid", nullable: false),
                    SpecializationId = table.Column<Guid>(type: "uuid", nullable: false),
                    RelevanceScore = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiseaseSpecializations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiseaseSpecializations_Diseases_DiseaseId",
                        column: x => x.DiseaseId,
                        principalTable: "Diseases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiseaseSpecializations_Specializations_SpecializationId",
                        column: x => x.SpecializationId,
                        principalSchema: "lookup",
                        principalTable: "Specializations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrescriptionTemplateItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PrescriptionTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    MedicationName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Strength = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Dosage = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Frequency = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Duration = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Route = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Timing = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Quantity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Instructions = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrescriptionTemplateItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrescriptionTemplateItems_PrescriptionTemplates_Prescriptio~",
                        column: x => x.PrescriptionTemplateId,
                        principalTable: "PrescriptionTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ExpiresAt_Status",
                schema: "clinical",
                table: "Appointments",
                columns: new[] { "ExpiresAt", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_Status",
                schema: "clinical",
                table: "Appointments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationTemplates_DoctorId_TemplateName",
                table: "ConsultationTemplates",
                columns: new[] { "DoctorId", "TemplateName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseSpecializations_DiseaseId",
                table: "DiseaseSpecializations",
                column: "DiseaseId");

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseSpecializations_SpecializationId",
                table: "DiseaseSpecializations",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorFavoriteMedicines_DoctorId_MedicationName",
                table: "DoctorFavoriteMedicines",
                columns: new[] { "DoctorId", "MedicationName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientFavoriteDoctors_DoctorId",
                table: "PatientFavoriteDoctors",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientFavoriteDoctors_PatientId_DoctorId",
                table: "PatientFavoriteDoctors",
                columns: new[] { "PatientId", "DoctorId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionTemplateItems_PrescriptionTemplateId",
                table: "PrescriptionTemplateItems",
                column: "PrescriptionTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionTemplates_DoctorId_TemplateName",
                table: "PrescriptionTemplates",
                columns: new[] { "DoctorId", "TemplateName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsultationTemplates");

            migrationBuilder.DropTable(
                name: "DiseaseSpecializations");

            migrationBuilder.DropTable(
                name: "DoctorFavoriteMedicines");

            migrationBuilder.DropTable(
                name: "PatientFavoriteDoctors");

            migrationBuilder.DropTable(
                name: "PrescriptionTemplateItems");

            migrationBuilder.DropTable(
                name: "Diseases");

            migrationBuilder.DropTable(
                name: "PrescriptionTemplates");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_ExpiresAt_Status",
                schema: "clinical",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_Status",
                schema: "clinical",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Gender",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PushToken",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Quantity",
                schema: "clinical",
                table: "PrescriptionItems");

            migrationBuilder.DropColumn(
                name: "Route",
                schema: "clinical",
                table: "PrescriptionItems");

            migrationBuilder.DropColumn(
                name: "Strength",
                schema: "clinical",
                table: "PrescriptionItems");

            migrationBuilder.DropColumn(
                name: "Timing",
                schema: "clinical",
                table: "PrescriptionItems");

            migrationBuilder.DropColumn(
                name: "ClinicName",
                schema: "clinical",
                table: "DoctorProfiles");

            migrationBuilder.DropColumn(
                name: "Languages",
                schema: "clinical",
                table: "DoctorProfiles");

            migrationBuilder.DropColumn(
                name: "FollowUpDate",
                schema: "clinical",
                table: "Consultations");

            migrationBuilder.DropColumn(
                name: "FollowUpInstructions",
                schema: "clinical",
                table: "Consultations");

            migrationBuilder.DropColumn(
                name: "FollowUpUrgency",
                schema: "clinical",
                table: "Consultations");

            migrationBuilder.DropColumn(
                name: "PatientNotes",
                schema: "clinical",
                table: "Consultations");

            migrationBuilder.DropColumn(
                name: "PrivateNotes",
                schema: "clinical",
                table: "Consultations");

            migrationBuilder.DropColumn(
                name: "VisitType",
                schema: "clinical",
                table: "Consultations");

            migrationBuilder.DropColumn(
                name: "DoctorNotes",
                schema: "clinical",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                schema: "clinical",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "SuggestedNewTime",
                schema: "clinical",
                table: "Appointments");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "clinical",
                table: "Appointments",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "CancellationReason",
                schema: "clinical",
                table: "Appointments",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BookingNote",
                schema: "clinical",
                table: "Appointments",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);
        }
    }
}
