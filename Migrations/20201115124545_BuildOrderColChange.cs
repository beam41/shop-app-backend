using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopAppBackend.Migrations
{
    public partial class BuildOrderColChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddressFullName",
                table: "BuildOrder",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProofOfPaymentDepositImage",
                table: "BuildOrder",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressFullName",
                table: "BuildOrder");

            migrationBuilder.DropColumn(
                name: "ProofOfPaymentDepositImage",
                table: "BuildOrder");
        }
    }
}
