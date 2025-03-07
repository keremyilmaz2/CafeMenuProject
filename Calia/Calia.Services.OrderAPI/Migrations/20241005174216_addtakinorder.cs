using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Calia.Services.OrderAPI.Migrations
{
    /// <inheritdoc />
    public partial class addtakinorder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "personTakingOrder",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "personTakingOrder",
                table: "OrderHeaders");
        }
    }
}
