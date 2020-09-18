using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopAppBackend.Migrations
{
    public partial class RemakeOrderFieldFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "State",
                table: "OrderStates",
                type: "varchar(33)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(MAX)");

            migrationBuilder.AddColumn<string>(
                name: "PurchaseMethod",
                table: "Order",
                type: "varchar(11)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurchaseMethod",
                table: "Order");

            migrationBuilder.AlterColumn<string>(
                name: "State",
                table: "OrderStates",
                type: "varchar(MAX)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(33)");
        }
    }
}
