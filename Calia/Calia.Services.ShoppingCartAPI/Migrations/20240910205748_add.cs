using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Calia.Services.ShoppingCartAPI.Migrations
{
    /// <inheritdoc />
    public partial class add : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductExtraDto");

            migrationBuilder.CreateTable(
                name: "ShoppingCartExtras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExtraName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    CartDetailsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCartExtras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShoppingCartExtras_CartDetails_CartDetailsId",
                        column: x => x.CartDetailsId,
                        principalTable: "CartDetails",
                        principalColumn: "CartDetailsId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCartExtras_CartDetailsId",
                table: "ShoppingCartExtras",
                column: "CartDetailsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShoppingCartExtras");

            migrationBuilder.CreateTable(
                name: "ProductExtraDto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CartDetailsId = table.Column<int>(type: "int", nullable: true),
                    ExtraName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductExtraDto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductExtraDto_CartDetails_CartDetailsId",
                        column: x => x.CartDetailsId,
                        principalTable: "CartDetails",
                        principalColumn: "CartDetailsId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductExtraDto_CartDetailsId",
                table: "ProductExtraDto",
                column: "CartDetailsId");
        }
    }
}
