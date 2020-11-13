using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopAppBackend.Migrations
{
    public partial class Remove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderStates_Order_OrderId",
                table: "OrderStates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderStates",
                table: "OrderStates");

            migrationBuilder.RenameTable(
                name: "OrderStates",
                newName: "OrderState");

            migrationBuilder.RenameIndex(
                name: "IX_OrderStates_OrderId",
                table: "OrderState",
                newName: "IX_OrderState_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderState",
                table: "OrderState",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderState_Order_OrderId",
                table: "OrderState",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderState_Order_OrderId",
                table: "OrderState");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderState",
                table: "OrderState");

            migrationBuilder.RenameTable(
                name: "OrderState",
                newName: "OrderStates");

            migrationBuilder.RenameIndex(
                name: "IX_OrderState_OrderId",
                table: "OrderStates",
                newName: "IX_OrderStates_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderStates",
                table: "OrderStates",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderStates_Order_OrderId",
                table: "OrderStates",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
