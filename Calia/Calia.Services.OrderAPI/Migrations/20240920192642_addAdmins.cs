using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Calia.Services.OrderAPI.Migrations
{
    /// <inheritdoc />
    public partial class addAdmins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminNames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdminName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminNames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KisiselGiders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdminId = table.Column<int>(type: "int", nullable: false),
                    AlinanPara = table.Column<double>(type: "float", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GiderTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KisiselGiders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KisiselGiders_AdminNames_AdminId",
                        column: x => x.AdminId,
                        principalTable: "AdminNames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AdminNames",
                columns: new[] { "Id", "AdminName" },
                values: new object[,]
                {
                    { 1, "Hasan" },
                    { 2, "Berke" },
                    { 3, "Mehmet" },
                    { 4, "Samet" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_KisiselGiders_AdminId",
                table: "KisiselGiders",
                column: "AdminId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KisiselGiders");

            migrationBuilder.DropTable(
                name: "AdminNames");
        }
    }
}
