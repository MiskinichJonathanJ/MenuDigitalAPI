using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRequiredAndNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DishID",
                table: "OrderItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_DishID",
                table: "OrderItems",
                column: "DishID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Dishes_DishID",
                table: "OrderItems",
                column: "DishID",
                principalTable: "Dishes",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Dishes_DishID",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_DishID",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "DishID",
                table: "OrderItems");
        }
    }
}
