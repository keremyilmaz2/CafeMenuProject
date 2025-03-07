using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Calia.Services.CategoryAPI.Migrations
{
    /// <inheritdoc />
    public partial class addCategoryToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    ProductCount = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "DisplayOrder", "Name", "ProductCount" },
                values: new object[,]
                {
                    { 1, 1, "Espresso Bar", false },
                    { 2, 2, "Brew Bar", false },
                    { 3, 3, "Hot Drinks", false },
                    { 4, 4, "Soft Drinks", false },
                    { 5, 5, "Ice Coffee", false },
                    { 6, 6, "Bitki Çayları", false },
                    { 7, 7, "Frozen", false },
                    { 8, 8, "Milkshake", false },
                    { 9, 9, "Smoothie", false },
                    { 10, 10, "Handmade", false },
                    { 11, 11, "Moctails", false },
                    { 12, 12, "Tatlılar", true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
