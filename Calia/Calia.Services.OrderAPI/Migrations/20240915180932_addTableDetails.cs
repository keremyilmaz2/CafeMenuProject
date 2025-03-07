using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Calia.Services.OrderAPI.Migrations
{
    /// <inheritdoc />
    public partial class addTableDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOccupied",
                table: "TableNos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CloseTime",
                table: "OrderHeaders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Ikram",
                table: "OrderHeaders",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Iskonto",
                table: "OrderHeaders",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "KrediKarti",
                table: "OrderHeaders",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Nakit",
                table: "OrderHeaders",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TableDetailsId",
                table: "OrderHeaders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "OrderDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isPaid",
                table: "OrderDetails",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "tableDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TableId = table.Column<int>(type: "int", nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ikram = table.Column<double>(type: "float", nullable: true),
                    Iskonto = table.Column<double>(type: "float", nullable: true),
                    Nakit = table.Column<double>(type: "float", nullable: true),
                    KrediKarti = table.Column<double>(type: "float", nullable: true),
                    TotalTable = table.Column<double>(type: "float", nullable: true),
                    OpenTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CloseTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tableDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tableDetails_TableNos_TableId",
                        column: x => x.TableId,
                        principalTable: "TableNos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "TableNos",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsOccupied",
                value: false);

            migrationBuilder.UpdateData(
                table: "TableNos",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsOccupied",
                value: false);

            migrationBuilder.UpdateData(
                table: "TableNos",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsOccupied",
                value: false);

            migrationBuilder.UpdateData(
                table: "TableNos",
                keyColumn: "Id",
                keyValue: 4,
                column: "IsOccupied",
                value: false);

            migrationBuilder.UpdateData(
                table: "TableNos",
                keyColumn: "Id",
                keyValue: 5,
                column: "IsOccupied",
                value: false);

            migrationBuilder.UpdateData(
                table: "TableNos",
                keyColumn: "Id",
                keyValue: 6,
                column: "IsOccupied",
                value: false);

            migrationBuilder.UpdateData(
                table: "TableNos",
                keyColumn: "Id",
                keyValue: 7,
                column: "IsOccupied",
                value: false);

            migrationBuilder.UpdateData(
                table: "TableNos",
                keyColumn: "Id",
                keyValue: 8,
                column: "IsOccupied",
                value: false);

            migrationBuilder.UpdateData(
                table: "TableNos",
                keyColumn: "Id",
                keyValue: 9,
                column: "IsOccupied",
                value: false);

            migrationBuilder.UpdateData(
                table: "TableNos",
                keyColumn: "Id",
                keyValue: 10,
                column: "IsOccupied",
                value: false);

            migrationBuilder.UpdateData(
                table: "TableNos",
                keyColumn: "Id",
                keyValue: 11,
                column: "IsOccupied",
                value: false);

            migrationBuilder.UpdateData(
                table: "TableNos",
                keyColumn: "Id",
                keyValue: 12,
                column: "IsOccupied",
                value: false);

            migrationBuilder.UpdateData(
                table: "TableNos",
                keyColumn: "Id",
                keyValue: 13,
                column: "IsOccupied",
                value: false);

            migrationBuilder.UpdateData(
                table: "TableNos",
                keyColumn: "Id",
                keyValue: 14,
                column: "IsOccupied",
                value: false);

            migrationBuilder.UpdateData(
                table: "TableNos",
                keyColumn: "Id",
                keyValue: 15,
                column: "IsOccupied",
                value: false);

            migrationBuilder.UpdateData(
                table: "TableNos",
                keyColumn: "Id",
                keyValue: 16,
                column: "IsOccupied",
                value: false);

            migrationBuilder.UpdateData(
                table: "TableNos",
                keyColumn: "Id",
                keyValue: 17,
                column: "IsOccupied",
                value: false);

            migrationBuilder.UpdateData(
                table: "TableNos",
                keyColumn: "Id",
                keyValue: 18,
                column: "IsOccupied",
                value: false);

            migrationBuilder.UpdateData(
                table: "TableNos",
                keyColumn: "Id",
                keyValue: 19,
                column: "IsOccupied",
                value: false);

            migrationBuilder.UpdateData(
                table: "TableNos",
                keyColumn: "Id",
                keyValue: 20,
                column: "IsOccupied",
                value: false);

            migrationBuilder.UpdateData(
                table: "TableNos",
                keyColumn: "Id",
                keyValue: 21,
                column: "IsOccupied",
                value: false);

            migrationBuilder.UpdateData(
                table: "TableNos",
                keyColumn: "Id",
                keyValue: 22,
                column: "IsOccupied",
                value: false);

            migrationBuilder.UpdateData(
                table: "TableNos",
                keyColumn: "Id",
                keyValue: 23,
                column: "IsOccupied",
                value: false);

            migrationBuilder.UpdateData(
                table: "TableNos",
                keyColumn: "Id",
                keyValue: 24,
                column: "IsOccupied",
                value: false);

            migrationBuilder.UpdateData(
                table: "TableNos",
                keyColumn: "Id",
                keyValue: 25,
                column: "IsOccupied",
                value: false);

            migrationBuilder.UpdateData(
                table: "TableNos",
                keyColumn: "Id",
                keyValue: 26,
                column: "IsOccupied",
                value: false);

            migrationBuilder.UpdateData(
                table: "TableNos",
                keyColumn: "Id",
                keyValue: 27,
                column: "IsOccupied",
                value: false);

            migrationBuilder.UpdateData(
                table: "TableNos",
                keyColumn: "Id",
                keyValue: 28,
                column: "IsOccupied",
                value: false);

            migrationBuilder.UpdateData(
                table: "TableNos",
                keyColumn: "Id",
                keyValue: 29,
                column: "IsOccupied",
                value: false);

            migrationBuilder.UpdateData(
                table: "TableNos",
                keyColumn: "Id",
                keyValue: 30,
                column: "IsOccupied",
                value: false);

            migrationBuilder.UpdateData(
                table: "TableNos",
                keyColumn: "Id",
                keyValue: 31,
                column: "IsOccupied",
                value: false);

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeaders_TableDetailsId",
                table: "OrderHeaders",
                column: "TableDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_tableDetails_TableId",
                table: "tableDetails",
                column: "TableId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHeaders_tableDetails_TableDetailsId",
                table: "OrderHeaders",
                column: "TableDetailsId",
                principalTable: "tableDetails",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderHeaders_tableDetails_TableDetailsId",
                table: "OrderHeaders");

            migrationBuilder.DropTable(
                name: "tableDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderHeaders_TableDetailsId",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "IsOccupied",
                table: "TableNos");

            migrationBuilder.DropColumn(
                name: "CloseTime",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "Ikram",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "Iskonto",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "KrediKarti",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "Nakit",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "TableDetailsId",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "isPaid",
                table: "OrderDetails");
        }
    }
}
