using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HouseHub.Migrations
{
    /// <inheritdoc />
    public partial class newBagContentAttribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BagId",
                table: "DndHouseHub",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DndHouseHub_BagId",
                table: "DndHouseHub",
                column: "BagId");

            migrationBuilder.AddForeignKey(
                name: "FK_DndHouseHub_BagsAPI_BagId",
                table: "DndHouseHub",
                column: "BagId",
                principalTable: "BagsAPI",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DndHouseHub_BagsAPI_BagId",
                table: "DndHouseHub");

            migrationBuilder.DropIndex(
                name: "IX_DndHouseHub_BagId",
                table: "DndHouseHub");

            migrationBuilder.DropColumn(
                name: "BagId",
                table: "DndHouseHub");
        }
    }
}
