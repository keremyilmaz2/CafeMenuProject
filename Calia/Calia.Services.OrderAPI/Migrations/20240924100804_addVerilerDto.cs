using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Calia.Services.OrderAPI.Migrations
{
    /// <inheritdoc />
    public partial class addVerilerDto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Veriler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalProductNumber = table.Column<int>(type: "int", nullable: false),
                    ToplamKazanç = table.Column<int>(type: "int", nullable: false),
                    ToplamSiparisSayisi = table.Column<int>(type: "int", nullable: false),
                    ToplamKapananMasaSayisi = table.Column<int>(type: "int", nullable: false),
                    HaftalikNakit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HaftalikKrediKarti = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AylikNakit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AylikKrediKarti = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SonGuncellemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SonGuncellemeTarihiYil = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veriler", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Veriler");
        }
    }
}
