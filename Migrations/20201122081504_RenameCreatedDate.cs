using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopAppBackend.Migrations
{
    public partial class RenameCreatedDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OrderState");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "OrderState",
                nullable: false,
                defaultValueSql: "getdate()");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "OrderState");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "OrderState",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "getdate()");
        }
    }
}
