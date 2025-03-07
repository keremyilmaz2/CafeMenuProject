using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Calia.Services.ShoppingCartAPI.Migrations
{
    /// <inheritdoc />
    public partial class addHasanaSelamlar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DetailPrice",
                table: "CartDetails",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DetailPrice",
                table: "CartDetails");
        }
    }
}
