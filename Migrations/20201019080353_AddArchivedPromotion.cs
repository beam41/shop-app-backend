using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopAppBackend.Migrations
{
    public partial class AddArchivedPromotion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Archived",
                table: "Promotion",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Archived",
                table: "Promotion");
        }
    }
}
