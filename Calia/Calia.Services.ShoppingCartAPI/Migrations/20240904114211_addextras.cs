using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Calia.Services.ShoppingCartAPI.Migrations
{
    /// <inheritdoc />
    public partial class addextras : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductExtraDto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExtraName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CartDetailsId = table.Column<int>(type: "int", nullable: true)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductExtraDto");
        }
    }
}
