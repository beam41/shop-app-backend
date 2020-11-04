using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopAppBackend.Migrations
{
    public partial class DistMethod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DistributionMethodId",
                table: "Order",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DistributionMethod",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Price = table.Column<double>(nullable: false),
                    Archived = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistributionMethod", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_DistributionMethodId",
                table: "Order",
                column: "DistributionMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_DistributionMethod_DistributionMethodId",
                table: "Order",
                column: "DistributionMethodId",
                principalTable: "DistributionMethod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_DistributionMethod_DistributionMethodId",
                table: "Order");

            migrationBuilder.DropTable(
                name: "DistributionMethod");

            migrationBuilder.DropIndex(
                name: "IX_Order_DistributionMethodId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "DistributionMethodId",
                table: "Order");
        }
    }
}
