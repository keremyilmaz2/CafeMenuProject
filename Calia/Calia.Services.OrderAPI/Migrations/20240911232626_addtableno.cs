using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Calia.Services.OrderAPI.Migrations
{
    /// <inheritdoc />
    public partial class addtableno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TableNos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MasaNo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableNos", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TableNos",
                columns: new[] { "Id", "MasaNo" },
                values: new object[,]
                {
                    { 1, "1" },
                    { 2, "2" },
                    { 3, "3" },
                    { 4, "4" },
                    { 5, "5" },
                    { 6, "6" },
                    { 7, "7" },
                    { 8, "8" },
                    { 9, "9" },
                    { 10, "10" },
                    { 11, "11" },
                    { 12, "12" },
                    { 13, "13" },
                    { 14, "14" },
                    { 15, "15" },
                    { 16, "16" },
                    { 17, "17" },
                    { 18, "18" },
                    { 19, "19" },
                    { 20, "20" },
                    { 21, "21" },
                    { 22, "22" },
                    { 23, "23" },
                    { 24, "24" },
                    { 25, "25" },
                    { 26, "26" },
                    { 27, "27" },
                    { 28, "28" },
                    { 29, "29" },
                    { 30, "30" },
                    { 31, "31" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TableNos");
        }
    }
}
