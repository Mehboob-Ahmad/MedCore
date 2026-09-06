using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicHp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDemoRequestImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VerificationDocumentsUrl",
                table: "DemoRequests",
                newName: "LicenseImageUrl");

            migrationBuilder.AddColumn<string>(
                name: "DegreeImageUrl",
                table: "DemoRequests",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DegreeImageUrl",
                table: "DemoRequests");

            migrationBuilder.RenameColumn(
                name: "LicenseImageUrl",
                table: "DemoRequests",
                newName: "VerificationDocumentsUrl");
        }
    }
}
