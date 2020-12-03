using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopAppBackend.Migrations
{
    public partial class Forgot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BuildOrderState_Order_OrderId",
                table: "BuildOrderState");

            migrationBuilder.DropIndex(
                name: "IX_BuildOrderState_OrderId",
                table: "BuildOrderState");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "BuildOrderState");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "BuildOrderState",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BuildOrderState_OrderId",
                table: "BuildOrderState",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_BuildOrderState_Order_OrderId",
                table: "BuildOrderState",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
