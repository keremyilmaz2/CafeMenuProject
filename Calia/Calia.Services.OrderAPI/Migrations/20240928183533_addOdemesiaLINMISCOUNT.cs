using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Calia.Services.OrderAPI.Migrations
{
    /// <inheritdoc />
    public partial class addOdemesiaLINMISCOUNT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OdemesiAlinmisCount",
                table: "OrderDetails",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OdemesiAlinmisCount",
                table: "OrderDetails");
        }
    }
}
