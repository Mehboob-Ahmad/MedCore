using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicHp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserVerificationOtp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastVerificationOtpRequestAt",
                schema: "core",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationOtp",
                schema: "core",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VerificationOtpAttempts",
                schema: "core",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerificationOtpExpiry",
                schema: "core",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastVerificationOtpRequestAt",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "VerificationOtp",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "VerificationOtpAttempts",
                schema: "core",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "VerificationOtpExpiry",
                schema: "core",
                table: "Users");
        }
    }
}
