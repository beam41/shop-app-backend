using Microsoft.EntityFrameworkCore.Migrations;

namespace ShopAppBackend.Migrations
{
    public partial class BuildOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_User_CreatedByUserId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_DistributionMethod_DistributionMethodId",
                table: "Order");

            migrationBuilder.AddColumn<int>(
                name: "BuildOrderId",
                table: "OrderState",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SubDistrict",
                table: "Order",
                type: "nvarchar(32)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Province",
                table: "Order",
                type: "nvarchar(32)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PostalCode",
                table: "Order",
                type: "char(5)",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(5)",
                oldMaxLength: 5,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Order",
                type: "char(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Order",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "District",
                table: "Order",
                type: "nvarchar(32)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DistributionMethodId",
                table: "Order",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CreatedByUserId",
                table: "Order",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Order",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "BuildOrder",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedByUserId = table.Column<int>(nullable: false),
                    DistributionMethodId = table.Column<int>(nullable: true),
                    FullName = table.Column<string>(nullable: false),
                    OrderDescription = table.Column<string>(nullable: false),
                    PhoneNumber = table.Column<string>(type: "char(10)", maxLength: 10, nullable: false),
                    Address = table.Column<string>(nullable: true),
                    Province = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    District = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    SubDistrict = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    PostalCode = table.Column<string>(type: "char(5)", maxLength: 5, nullable: true),
                    ProofOfPaymentFullImage = table.Column<string>(nullable: true),
                    TrackingNumber = table.Column<string>(nullable: true),
                    ReceivedMessage = table.Column<string>(nullable: true),
                    CancelledByAdmin = table.Column<bool>(nullable: true),
                    CancelledReason = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuildOrder_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuildOrder_DistributionMethod_DistributionMethodId",
                        column: x => x.DistributionMethodId,
                        principalTable: "DistributionMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BuildOrderImage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BuildOrderId = table.Column<int>(nullable: false),
                    ImageFileName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildOrderImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuildOrderImage_BuildOrder_BuildOrderId",
                        column: x => x.BuildOrderId,
                        principalTable: "BuildOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderState_BuildOrderId",
                table: "OrderState",
                column: "BuildOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildOrder_CreatedByUserId",
                table: "BuildOrder",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildOrder_DistributionMethodId",
                table: "BuildOrder",
                column: "DistributionMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildOrderImage_BuildOrderId",
                table: "BuildOrderImage",
                column: "BuildOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_User_CreatedByUserId",
                table: "Order",
                column: "CreatedByUserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_DistributionMethod_DistributionMethodId",
                table: "Order",
                column: "DistributionMethodId",
                principalTable: "DistributionMethod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderState_BuildOrder_BuildOrderId",
                table: "OrderState",
                column: "BuildOrderId",
                principalTable: "BuildOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_User_CreatedByUserId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_DistributionMethod_DistributionMethodId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderState_BuildOrder_BuildOrderId",
                table: "OrderState");

            migrationBuilder.DropTable(
                name: "BuildOrderImage");

            migrationBuilder.DropTable(
                name: "BuildOrder");

            migrationBuilder.DropIndex(
                name: "IX_OrderState_BuildOrderId",
                table: "OrderState");

            migrationBuilder.DropColumn(
                name: "BuildOrderId",
                table: "OrderState");

            migrationBuilder.AlterColumn<string>(
                name: "SubDistrict",
                table: "Order",
                type: "nvarchar(32)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)");

            migrationBuilder.AlterColumn<string>(
                name: "Province",
                table: "Order",
                type: "nvarchar(32)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)");

            migrationBuilder.AlterColumn<string>(
                name: "PostalCode",
                table: "Order",
                type: "char(5)",
                maxLength: 5,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(5)",
                oldMaxLength: 5);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Order",
                type: "char(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "District",
                table: "Order",
                type: "nvarchar(32)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)");

            migrationBuilder.AlterColumn<int>(
                name: "DistributionMethodId",
                table: "Order",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "CreatedByUserId",
                table: "Order",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddForeignKey(
                name: "FK_Order_User_CreatedByUserId",
                table: "Order",
                column: "CreatedByUserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_DistributionMethod_DistributionMethodId",
                table: "Order",
                column: "DistributionMethodId",
                principalTable: "DistributionMethod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
