using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopAppBackend.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DistributionMethod",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Price = table.Column<double>(nullable: false),
                    Archived = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistributionMethod", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethod",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Bank = table.Column<string>(nullable: true),
                    AccountNumber = table.Column<string>(nullable: true),
                    AccountName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethod", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    Archived = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Promotion",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    IsBroadcasted = table.Column<bool>(nullable: false),
                    Archived = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    Password = table.Column<string>(type: "char(44)", maxLength: 44, nullable: false),
                    PhoneNumber = table.Column<string>(type: "char(10)", maxLength: 10, nullable: false),
                    FullName = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Province = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    District = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    SubDistrict = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    PostalCode = table.Column<string>(type: "char(5)", maxLength: 5, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    IsVisible = table.Column<bool>(nullable: false),
                    Archived = table.Column<bool>(nullable: false, defaultValue: false),
                    TypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_ProductType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "ProductType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuildOrder",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreatedByUserId = table.Column<int>(nullable: false),
                    DistributionMethodId = table.Column<int>(nullable: true),
                    FullName = table.Column<string>(nullable: false),
                    OrderDescription = table.Column<string>(nullable: false),
                    PhoneNumber = table.Column<string>(type: "char(10)", maxLength: 10, nullable: false),
                    AddressPhoneNumber = table.Column<string>(type: "char(10)", maxLength: 10, nullable: true),
                    AddressFullName = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Province = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    District = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    SubDistrict = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    PostalCode = table.Column<string>(type: "char(5)", maxLength: 5, nullable: true),
                    ProofOfPaymentFullImage = table.Column<string>(nullable: true),
                    ProofOfPaymentDepositImage = table.Column<string>(nullable: true),
                    TrackingNumber = table.Column<string>(nullable: true),
                    ReceivedMessage = table.Column<string>(nullable: true),
                    CancelledByAdmin = table.Column<bool>(nullable: true),
                    CancelledReason = table.Column<string>(nullable: true),
                    DepositPrice = table.Column<double>(nullable: true),
                    FullPrice = table.Column<double>(nullable: true),
                    ExpectedCompleteDate = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuildOrder_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuildOrder_DistributionMethod_DistributionMethodId",
                        column: x => x.DistributionMethodId,
                        principalTable: "DistributionMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreatedByUserId = table.Column<int>(nullable: false),
                    PurchaseMethod = table.Column<string>(type: "varchar(11)", nullable: false),
                    DistributionMethodId = table.Column<int>(nullable: false),
                    TrackingNumber = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(type: "char(10)", maxLength: 10, nullable: false),
                    FullName = table.Column<string>(nullable: false),
                    Address = table.Column<string>(nullable: false),
                    Province = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    District = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    SubDistrict = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    PostalCode = table.Column<string>(type: "char(5)", maxLength: 5, nullable: false),
                    ProofOfPaymentFullImage = table.Column<string>(nullable: true),
                    ReceivedMessage = table.Column<string>(nullable: true),
                    CancelledByAdmin = table.Column<bool>(nullable: true),
                    CancelledReason = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Order_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Order_DistributionMethod_DistributionMethodId",
                        column: x => x.DistributionMethodId,
                        principalTable: "DistributionMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductImage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<string>(nullable: false),
                    ImageFileName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductImage_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromotionItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PromotionId = table.Column<int>(nullable: false),
                    InPromotionProductId = table.Column<string>(nullable: false),
                    NewPrice = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionItem_Product_InPromotionProductId",
                        column: x => x.InPromotionProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PromotionItem_Promotion_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "Promotion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuildOrderImage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BuildOrderId = table.Column<string>(nullable: false),
                    ImageFileName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildOrderImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuildOrderImage_BuildOrder_BuildOrderId",
                        column: x => x.BuildOrderId,
                        principalTable: "BuildOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuildOrderState",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    State = table.Column<string>(type: "varchar(33)", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "getdate()"),
                    BuildOrderId = table.Column<string>(nullable: true)
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
                });

            migrationBuilder.CreateTable(
                name: "OrderProduct",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<string>(nullable: true),
                    ProductId = table.Column<string>(nullable: true),
                    Amount = table.Column<int>(nullable: false),
                    SavedPrice = table.Column<double>(nullable: false),
                    SavedNewPrice = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderProduct_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderProduct_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderState",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    State = table.Column<string>(type: "varchar(33)", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "getdate()"),
                    OrderId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderState", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderState_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BuildOrder_CreatedByUserId",
                table: "BuildOrder",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildOrder_DistributionMethodId",
                table: "BuildOrder",
                column: "DistributionMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildOrderImage_BuildOrderId",
                table: "BuildOrderImage",
                column: "BuildOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildOrderState_BuildOrderId",
                table: "BuildOrderState",
                column: "BuildOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_CreatedByUserId",
                table: "Order",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_DistributionMethodId",
                table: "Order",
                column: "DistributionMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProduct_OrderId",
                table: "OrderProduct",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProduct_ProductId",
                table: "OrderProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderState_OrderId",
                table: "OrderState",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_TypeId",
                table: "Product",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImage_ProductId",
                table: "ProductImage",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionItem_InPromotionProductId",
                table: "PromotionItem",
                column: "InPromotionProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionItem_PromotionId",
                table: "PromotionItem",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Username",
                table: "User",
                column: "Username",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuildOrderImage");

            migrationBuilder.DropTable(
                name: "BuildOrderState");

            migrationBuilder.DropTable(
                name: "OrderProduct");

            migrationBuilder.DropTable(
                name: "OrderState");

            migrationBuilder.DropTable(
                name: "PaymentMethod");

            migrationBuilder.DropTable(
                name: "ProductImage");

            migrationBuilder.DropTable(
                name: "PromotionItem");

            migrationBuilder.DropTable(
                name: "BuildOrder");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "Promotion");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "DistributionMethod");

            migrationBuilder.DropTable(
                name: "ProductType");
        }
    }
}
