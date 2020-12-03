using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopAppBackend.Migrations
{
    public partial class FixDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PromotionItem_Product_InPromotionProductId",
                table: "PromotionItem");

            migrationBuilder.AddForeignKey(
                name: "FK_PromotionItem_Product_InPromotionProductId",
                table: "PromotionItem",
                column: "InPromotionProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PromotionItem_Product_InPromotionProductId",
                table: "PromotionItem");

            migrationBuilder.AddForeignKey(
                name: "FK_PromotionItem_Product_InPromotionProductId",
                table: "PromotionItem",
                column: "InPromotionProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
