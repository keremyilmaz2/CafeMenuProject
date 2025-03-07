using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Calia.Services.OrderAPI.Migrations
{
    /// <inheritdoc />
    public partial class addCancelDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CancelDetailsId",
                table: "OrderExtras",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CancelDetails",
                columns: table => new
                {
                    CancelDetailsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderHeaderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CancelTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    isPaid = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CancelDetails", x => x.CancelDetailsId);
                    table.ForeignKey(
                        name: "FK_CancelDetails_OrderHeaders_OrderHeaderId",
                        column: x => x.OrderHeaderId,
                        principalTable: "OrderHeaders",
                        principalColumn: "OrderHeaderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderExtras_CancelDetailsId",
                table: "OrderExtras",
                column: "CancelDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_CancelDetails_OrderHeaderId",
                table: "CancelDetails",
                column: "OrderHeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderExtras_CancelDetails_CancelDetailsId",
                table: "OrderExtras",
                column: "CancelDetailsId",
                principalTable: "CancelDetails",
                principalColumn: "CancelDetailsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderExtras_CancelDetails_CancelDetailsId",
                table: "OrderExtras");

            migrationBuilder.DropTable(
                name: "CancelDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderExtras_CancelDetailsId",
                table: "OrderExtras");

            migrationBuilder.DropColumn(
                name: "CancelDetailsId",
                table: "OrderExtras");
        }
    }
}
