using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopAppBackend.Migrations
{
    public partial class ReduceJson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StateDataJson",
                table: "OrderStates");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CancelledByAdmin",
                table: "Order",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CancelledReason",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "Order",
                type: "nvarchar(32)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Order",
                type: "char(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Order",
                type: "char(5)",
                maxLength: 5,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProofOfPaymentFullImage",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "Order",
                type: "nvarchar(32)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceivedMessage",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubDistrict",
                table: "Order",
                type: "nvarchar(32)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrackingNumber",
                table: "Order",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "CancelledByAdmin",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "CancelledReason",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "District",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ProofOfPaymentFullImage",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Province",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ReceivedMessage",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "SubDistrict",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "TrackingNumber",
                table: "Order");

            migrationBuilder.AddColumn<string>(
                name: "StateDataJson",
                table: "OrderStates",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
