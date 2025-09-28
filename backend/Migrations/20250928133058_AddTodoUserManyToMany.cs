using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace familyHub.Migrations
{
    /// <inheritdoc />
    public partial class AddTodoUserManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TodoUsersAPI",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TodoId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoUsersAPI", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TodoUsersAPI_TodosAPI_TodoId",
                        column: x => x.TodoId,
                        principalTable: "TodosAPI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TodoUsersAPI_UsersAPI_UserId",
                        column: x => x.UserId,
                        principalTable: "UsersAPI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TodoUsersAPI_TodoId_UserId",
                table: "TodoUsersAPI",
                columns: new[] { "TodoId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TodoUsersAPI_UserId",
                table: "TodoUsersAPI",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TodoUsersAPI");
        }
    }
}
