using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopAppBackend.Migrations
{
    public partial class BuildOrderStateAndOnDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_DistributionMethod_DistributionMethodId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderProduct_Order_OrderId",
                table: "OrderProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderState_BuildOrder_BuildOrderId",
                table: "OrderState");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderState_Order_OrderId",
                table: "OrderState");

            migrationBuilder.DropIndex(
                name: "IX_OrderState_BuildOrderId",
                table: "OrderState");

            migrationBuilder.DropColumn(
                name: "BuildOrderId",
                table: "OrderState");

            migrationBuilder.CreateTable(
                name: "BuildOrderState",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    State = table.Column<string>(type: "varchar(33)", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    OrderId = table.Column<int>(nullable: true),
                    BuildOrderId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildOrderState", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuildOrderState_BuildOrder_BuildOrderId",
                        column: x => x.BuildOrderId,
                        principalTable: "BuildOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuildOrderState_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BuildOrderState_BuildOrderId",
                table: "BuildOrderState",
                column: "BuildOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildOrderState_OrderId",
                table: "BuildOrderState",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_DistributionMethod_DistributionMethodId",
                table: "Order",
                column: "DistributionMethodId",
                principalTable: "DistributionMethod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProduct_Order_OrderId",
                table: "OrderProduct",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderState_Order_OrderId",
                table: "OrderState",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_DistributionMethod_DistributionMethodId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderProduct_Order_OrderId",
                table: "OrderProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderState_Order_OrderId",
                table: "OrderState");

            migrationBuilder.DropTable(
                name: "BuildOrderState");

            migrationBuilder.AddColumn<int>(
                name: "BuildOrderId",
                table: "OrderState",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderState_BuildOrderId",
                table: "OrderState",
                column: "BuildOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_DistributionMethod_DistributionMethodId",
                table: "Order",
                column: "DistributionMethodId",
                principalTable: "DistributionMethod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProduct_Order_OrderId",
                table: "OrderProduct",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderState_BuildOrder_BuildOrderId",
                table: "OrderState",
                column: "BuildOrderId",
                principalTable: "BuildOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderState_Order_OrderId",
                table: "OrderState",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
