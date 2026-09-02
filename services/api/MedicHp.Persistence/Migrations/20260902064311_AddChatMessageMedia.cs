using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicHp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddChatMessageMedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Content",
                schema: "messaging",
                table: "ChatMessages",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "AttachmentId",
                schema: "messaging",
                table: "ChatMessages",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MessageType",
                schema: "messaging",
                table: "ChatMessages",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_AttachmentId",
                schema: "messaging",
                table: "ChatMessages",
                column: "AttachmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Files_AttachmentId",
                schema: "messaging",
                table: "ChatMessages",
                column: "AttachmentId",
                principalSchema: "core",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Files_AttachmentId",
                schema: "messaging",
                table: "ChatMessages");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_AttachmentId",
                schema: "messaging",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "AttachmentId",
                schema: "messaging",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "MessageType",
                schema: "messaging",
                table: "ChatMessages");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                schema: "messaging",
                table: "ChatMessages",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
