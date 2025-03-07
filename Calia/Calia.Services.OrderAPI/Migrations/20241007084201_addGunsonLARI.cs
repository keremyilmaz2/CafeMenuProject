using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Calia.Services.OrderAPI.Migrations
{
    /// <inheritdoc />
    public partial class addGunsonLARI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GunSonlaris",
                columns: table => new
                {
                    GunSonlariId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GunlukNakit = table.Column<double>(type: "float", nullable: false),
                    GunlukKrediKarti = table.Column<double>(type: "float", nullable: false),
                    GunlukIskonto = table.Column<double>(type: "float", nullable: false),
                    GunlukIkram = table.Column<double>(type: "float", nullable: false),
                    GunlukGider = table.Column<double>(type: "float", nullable: false),
                    GununTarih = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GunSonlaris", x => x.GunSonlariId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GunSonlaris");
        }
    }
}
