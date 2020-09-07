using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopAppBackend.Migrations
{
    public partial class RenameProductImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "ProductImage");

            migrationBuilder.AddColumn<string>(
                name: "ImageFileName",
                table: "ProductImage",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageFileName",
                table: "ProductImage");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "ProductImage",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
