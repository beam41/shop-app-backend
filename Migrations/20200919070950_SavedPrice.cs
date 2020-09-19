using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopAppBackend.Migrations
{
    public partial class SavedPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "NewPrice",
                table: "PromotionItem",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "Product",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<double>(
                name: "SavedNewPrice",
                table: "OrderProduct",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SavedPrice",
                table: "OrderProduct",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SavedNewPrice",
                table: "OrderProduct");

            migrationBuilder.DropColumn(
                name: "SavedPrice",
                table: "OrderProduct");

            migrationBuilder.AlterColumn<int>(
                name: "NewPrice",
                table: "PromotionItem",
                type: "int",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<int>(
                name: "Price",
                table: "Product",
                type: "int",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}
