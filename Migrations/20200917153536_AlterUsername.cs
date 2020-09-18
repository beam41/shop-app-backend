using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopAppBackend.Migrations
{
    public partial class AlterUsername : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_User_Username",
                table: "User");

            migrationBuilder.CreateIndex(
                name: "IX_User_Username",
                table: "User",
                column: "Username",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_Username",
                table: "User");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_User_Username",
                table: "User",
                column: "Username");
        }
    }
}
