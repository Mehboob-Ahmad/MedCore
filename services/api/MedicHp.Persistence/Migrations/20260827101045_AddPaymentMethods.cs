using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicHp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentMethods : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LicenseNumber",
                schema: "clinical",
                table: "DoctorProfiles",
                newName: "RegistrationNumber");

            migrationBuilder.AddColumn<string>(
                name: "Achievements",
                schema: "clinical",
                table: "DoctorProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Awards",
                schema: "clinical",
                table: "DoctorProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfessionalType",
                schema: "clinical",
                table: "DoctorProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RegulatoryBody",
                schema: "clinical",
                table: "DoctorProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerificationDate",
                schema: "clinical",
                table: "DoctorProfiles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationDocumentUrl",
                schema: "clinical",
                table: "DoctorProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationNotes",
                schema: "clinical",
                table: "DoctorProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "WhatsAppEnabled",
                schema: "clinical",
                table: "DoctorProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "WhatsAppNumber",
                schema: "clinical",
                table: "DoctorProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AppointmentReminderSentAt",
                schema: "clinical",
                table: "Appointments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentConfirmedAt",
                schema: "clinical",
                table: "Appointments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentConfirmedByUserId",
                schema: "clinical",
                table: "Appointments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentOverdueNotifiedAt",
                schema: "clinical",
                table: "Appointments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentReminderSentAt",
                schema: "clinical",
                table: "Appointments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                schema: "clinical",
                table: "Appointments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "DoctorCertifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DoctorProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IssuingOrganization = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorCertifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoctorCertifications_DoctorProfiles_DoctorProfileId",
                        column: x => x.DoctorProfileId,
                        principalSchema: "clinical",
                        principalTable: "DoctorProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DoctorPaymentMethods",
                schema: "clinical",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    DoctorProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentMethodType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PaymentProvider = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    AccountTitle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    AccountNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IBAN = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorPaymentMethods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoctorPaymentMethods_DoctorProfiles_DoctorProfileId",
                        column: x => x.DoctorProfileId,
                        principalSchema: "clinical",
                        principalTable: "DoctorProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DoctorQualifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DoctorProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Degree = table.Column<string>(type: "text", nullable: false),
                    Institution = table.Column<string>(type: "text", nullable: false),
                    CompletionYear = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorQualifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoctorQualifications_DoctorProfiles_DoctorProfileId",
                        column: x => x.DoctorProfileId,
                        principalSchema: "clinical",
                        principalTable: "DoctorProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WhatsAppMessages",
                schema: "messaging",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WhatsAppMessageId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    MessageType = table.Column<int>(type: "integer", nullable: false),
                    Direction = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StatusTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ErrorCode = table.Column<int>(type: "integer", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Metadata = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    DoctorProfileId = table.Column<Guid>(type: "uuid", nullable: true),
                    RecipientPhoneNumber = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhatsAppMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WhatsAppMessages_DoctorProfiles_DoctorProfileId",
                        column: x => x.DoctorProfileId,
                        principalSchema: "clinical",
                        principalTable: "DoctorProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_WhatsAppMessages_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "core",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DoctorCertifications_DoctorProfileId",
                table: "DoctorCertifications",
                column: "DoctorProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorPaymentMethods_DoctorProfileId_IsActive",
                schema: "clinical",
                table: "DoctorPaymentMethods",
                columns: new[] { "DoctorProfileId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_DoctorQualifications_DoctorProfileId",
                table: "DoctorQualifications",
                column: "DoctorProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_WhatsAppMessages_Direction",
                schema: "messaging",
                table: "WhatsAppMessages",
                column: "Direction");

            migrationBuilder.CreateIndex(
                name: "IX_WhatsAppMessages_DoctorProfileId",
                schema: "messaging",
                table: "WhatsAppMessages",
                column: "DoctorProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_WhatsAppMessages_PhoneNumber",
                schema: "messaging",
                table: "WhatsAppMessages",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_WhatsAppMessages_Status",
                schema: "messaging",
                table: "WhatsAppMessages",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WhatsAppMessages_UserId",
                schema: "messaging",
                table: "WhatsAppMessages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WhatsAppMessages_WhatsAppMessageId",
                schema: "messaging",
                table: "WhatsAppMessages",
                column: "WhatsAppMessageId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DoctorCertifications");

            migrationBuilder.DropTable(
                name: "DoctorPaymentMethods",
                schema: "clinical");

            migrationBuilder.DropTable(
                name: "DoctorQualifications");

            migrationBuilder.DropTable(
                name: "WhatsAppMessages",
                schema: "messaging");

            migrationBuilder.DropColumn(
                name: "Achievements",
                schema: "clinical",
                table: "DoctorProfiles");

            migrationBuilder.DropColumn(
                name: "Awards",
                schema: "clinical",
                table: "DoctorProfiles");

            migrationBuilder.DropColumn(
                name: "ProfessionalType",
                schema: "clinical",
                table: "DoctorProfiles");

            migrationBuilder.DropColumn(
                name: "RegulatoryBody",
                schema: "clinical",
                table: "DoctorProfiles");

            migrationBuilder.DropColumn(
                name: "VerificationDate",
                schema: "clinical",
                table: "DoctorProfiles");

            migrationBuilder.DropColumn(
                name: "VerificationDocumentUrl",
                schema: "clinical",
                table: "DoctorProfiles");

            migrationBuilder.DropColumn(
                name: "VerificationNotes",
                schema: "clinical",
                table: "DoctorProfiles");

            migrationBuilder.DropColumn(
                name: "WhatsAppEnabled",
                schema: "clinical",
                table: "DoctorProfiles");

            migrationBuilder.DropColumn(
                name: "WhatsAppNumber",
                schema: "clinical",
                table: "DoctorProfiles");

            migrationBuilder.DropColumn(
                name: "AppointmentReminderSentAt",
                schema: "clinical",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PaymentConfirmedAt",
                schema: "clinical",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PaymentConfirmedByUserId",
                schema: "clinical",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PaymentOverdueNotifiedAt",
                schema: "clinical",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PaymentReminderSentAt",
                schema: "clinical",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                schema: "clinical",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "RegistrationNumber",
                schema: "clinical",
                table: "DoctorProfiles",
                newName: "LicenseNumber");
        }
    }
}
