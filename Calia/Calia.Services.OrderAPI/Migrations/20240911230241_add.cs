using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Calia.Services.OrderAPI.Migrations
{
    /// <inheritdoc />
    public partial class add : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderExtra_OrderDetails_OrderDetailsId",
                table: "OrderExtra");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderExtra",
                table: "OrderExtra");

            migrationBuilder.RenameTable(
                name: "OrderExtra",
                newName: "OrderExtras");

            migrationBuilder.RenameIndex(
                name: "IX_OrderExtra_OrderDetailsId",
                table: "OrderExtras",
                newName: "IX_OrderExtras_OrderDetailsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderExtras",
                table: "OrderExtras",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderExtras_OrderDetails_OrderDetailsId",
                table: "OrderExtras",
                column: "OrderDetailsId",
                principalTable: "OrderDetails",
                principalColumn: "OrderDetailsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderExtras_OrderDetails_OrderDetailsId",
                table: "OrderExtras");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderExtras",
                table: "OrderExtras");

            migrationBuilder.RenameTable(
                name: "OrderExtras",
                newName: "OrderExtra");

            migrationBuilder.RenameIndex(
                name: "IX_OrderExtras_OrderDetailsId",
                table: "OrderExtra",
                newName: "IX_OrderExtra_OrderDetailsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderExtra",
                table: "OrderExtra",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderExtra_OrderDetails_OrderDetailsId",
                table: "OrderExtra",
                column: "OrderDetailsId",
                principalTable: "OrderDetails",
                principalColumn: "OrderDetailsId");
        }
    }
}
