using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopAppBackend.Migrations
{
    public partial class AddOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedByUserId = table.Column<int>(nullable: true),
                    PhoneNumber = table.Column<string>(type: "char(10)", maxLength: 10, nullable: false),
                    FullName = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Province = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    District = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    SubDistrict = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    PostalCode = table.Column<string>(type: "char(5)", maxLength: 5, nullable: true),
                    PurchaseMethod = table.Column<string>(type: "varchar(MAX)", nullable: false),
                    ProofOfPaymentImgPath = table.Column<string>(nullable: true),
                    DistributionMethod = table.Column<string>(type: "varchar(MAX)", nullable: false),
                    ParcelNumber = table.Column<string>(nullable: true),
                    IsCancelledBySeller = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Order_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderProduct",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(nullable: true),
                    ProductId = table.Column<int>(nullable: true),
                    Amount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderProduct_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderProduct_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderPromotion",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(nullable: true),
                    PromotionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPromotion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderPromotion_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderPromotion_Promotion_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "Promotion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderStates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    State = table.Column<string>(type: "varchar(MAX)", nullable: false),
                    OrderId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderStates_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_CreatedByUserId",
                table: "Order",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProduct_OrderId",
                table: "OrderProduct",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProduct_ProductId",
                table: "OrderProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPromotion_OrderId",
                table: "OrderPromotion",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPromotion_PromotionId",
                table: "OrderPromotion",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderStates_OrderId",
                table: "OrderStates",
                column: "OrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderProduct");

            migrationBuilder.DropTable(
                name: "OrderPromotion");

            migrationBuilder.DropTable(
                name: "OrderStates");

            migrationBuilder.DropTable(
                name: "Order");
        }
    }
}
