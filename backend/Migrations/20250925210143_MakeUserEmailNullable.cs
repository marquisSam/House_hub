using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HouseHub.Migrations
{
    /// <inheritdoc />
    public partial class MakeUserEmailNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UsersAPI_Email",
                table: "UsersAPI");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "UsersAPI",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.CreateIndex(
                name: "IX_UsersAPI_Email",
                table: "UsersAPI",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UsersAPI_Email",
                table: "UsersAPI");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "UsersAPI",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsersAPI_Email",
                table: "UsersAPI",
                column: "Email",
                unique: true);
        }
    }
}
