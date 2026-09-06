using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicHp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDemoAccountToDoctor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDemoAccount",
                schema: "clinical",
                table: "DoctorProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDemoAccount",
                schema: "clinical",
                table: "DoctorProfiles");
        }
    }
}
