using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicHp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountStatusAndDemoRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_IsActive",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "core",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "AccountStatus",
                schema: "core",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DemoRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    Specialization = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    ClinicOrHospital = table.Column<string>(type: "text", nullable: false),
                    YearsOfExperience = table.Column<int>(type: "integer", nullable: false),
                    ProfessionalQualification = table.Column<string>(type: "text", nullable: false),
                    AdditionalInformation = table.Column<string>(type: "text", nullable: true),
                    VerificationDocumentsUrl = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemoRequests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_AccountStatus",
                schema: "core",
                table: "Users",
                column: "AccountStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DemoRequests");

            migrationBuilder.DropIndex(
                name: "IX_Users_AccountStatus",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AccountStatus",
                schema: "core",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "core",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsActive",
                schema: "core",
                table: "Users",
                column: "IsActive");
        }
    }
}
