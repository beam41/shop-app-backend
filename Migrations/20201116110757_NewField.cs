using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopAppBackend.Migrations
{
    public partial class NewField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DepositPrice",
                table: "BuildOrder",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ExpectedCompleteDate",
                table: "BuildOrder",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FullPrice",
                table: "BuildOrder",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepositPrice",
                table: "BuildOrder");

            migrationBuilder.DropColumn(
                name: "ExpectedCompleteDate",
                table: "BuildOrder");

            migrationBuilder.DropColumn(
                name: "FullPrice",
                table: "BuildOrder");
        }
    }
}
