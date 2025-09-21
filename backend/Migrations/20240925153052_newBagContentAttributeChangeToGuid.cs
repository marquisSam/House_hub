using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HouseHub.Migrations
{
    /// <inheritdoc />
    public partial class newBagContentAttributeChangeToGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "BagsAPI",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "BagsAPI");

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
    }
}
