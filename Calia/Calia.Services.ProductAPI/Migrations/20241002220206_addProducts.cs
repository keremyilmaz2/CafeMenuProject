using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Calia.Services.ProductAPI.Migrations
{
    /// <inheritdoc />
    public partial class addProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "AvailableProducts", "CategoryId", "Name", "Price" },
                values: new object[,]
                {
                    { 1, 0, 1, "Espresso", 70.0 },
                    { 2, 0, 1, "Double Espresso", 90.0 },
                    { 3, 0, 1, "Espresso Macchiato", 75.0 },
                    { 4, 0, 1, "Americano", 95.0 },
                    { 5, 0, 1, "Latte", 100.0 },
                    { 6, 0, 1, "Flat White", 100.0 },
                    { 7, 0, 1, "Cappuccino", 100.0 },
                    { 8, 0, 1, "Cortado", 100.0 },
                    { 9, 0, 1, "Caramel Macchiato", 110.0 },
                    { 10, 0, 1, "Mocha", 100.0 },
                    { 11, 0, 1, "White Mocha", 110.0 },
                    { 12, 0, 1, "Zebra Mocha", 110.0 },
                    { 13, 0, 1, "Lotus Latte", 110.0 },
                    { 14, 0, 1, "Pumpkin Spice Latte", 110.0 },
                    { 15, 0, 2, "V60", 110.0 },
                    { 16, 0, 2, "Chemex", 110.0 },
                    { 17, 0, 2, "Filtre Kahve", 90.0 },
                    { 18, 0, 2, "Black Eye", 110.0 },
                    { 19, 0, 2, "Red Eye", 100.0 },
                    { 20, 0, 2, "Death Eye", 120.0 },
                    { 21, 0, 3, "Çay", 30.0 },
                    { 22, 0, 3, "Fincan Çay", 45.0 },
                    { 23, 0, 3, "Sütlü Çay", 65.0 },
                    { 24, 0, 3, "Türk Kahvesi", 70.0 },
                    { 25, 0, 3, "Double Türk Kahvesi", 90.0 },
                    { 26, 0, 3, "Sütlü Menengiç", 75.0 },
                    { 27, 0, 3, "Menengiç Kahvesi", 75.0 },
                    { 28, 0, 3, "Sıcak Çikolata", 85.0 },
                    { 29, 0, 3, "Beyaz Çikolata", 85.0 },
                    { 30, 0, 3, "Salep", 85.0 },
                    { 31, 0, 3, "Chai Tea Latte", 105.0 },
                    { 32, 0, 3, "Ballı Süt", 70.0 },
                    { 33, 0, 4, "Su", 30.0 },
                    { 34, 0, 4, "Soda", 50.0 },
                    { 35, 0, 4, "Meyveli Soda", 55.0 },
                    { 36, 0, 4, "Churchill", 60.0 },
                    { 37, 0, 4, "Vişne Soda", 70.0 },
                    { 38, 0, 4, "Red Bull", 90.0 },
                    { 39, 0, 4, "Detox", 100.0 },
                    { 40, 0, 5, "Ice Americano", 100.0 },
                    { 41, 0, 5, "Ice Filtre Kahve", 100.0 },
                    { 42, 0, 5, "Ice Latte", 110.0 },
                    { 43, 0, 5, "Ice Mocha", 120.0 },
                    { 44, 0, 5, "Ice White Mocha", 120.0 },
                    { 45, 0, 5, "Ice Zebra Mocha", 120.0 },
                    { 46, 0, 5, "Ice Red Eye", 110.0 },
                    { 47, 0, 5, "Ice Black Eye", 105.0 },
                    { 48, 0, 5, "Ice Lotus Latte", 120.0 },
                    { 49, 0, 5, "Ice Cold Brew Latte", 130.0 },
                    { 50, 0, 5, "Frappe", 120.0 },
                    { 51, 0, 5, "Affagato", 120.0 },
                    { 52, 0, 5, "Freddo Cappuccino", 130.0 },
                    { 53, 0, 5, "Cold Brew", 120.0 },
                    { 54, 0, 6, "Ihlamur", 80.0 },
                    { 55, 0, 6, "Yeşil Çay", 80.0 },
                    { 56, 0, 6, "Yaseminli Yeşil Çay", 80.0 },
                    { 57, 0, 6, "Kış Çayı", 80.0 },
                    { 58, 0, 6, "Ada Çayı", 80.0 },
                    { 59, 0, 6, "Hibiscus", 80.0 },
                    { 60, 0, 7, "Çilek", 100.0 },
                    { 61, 0, 7, "Blueberry", 100.0 },
                    { 62, 0, 7, "Mango", 100.0 },
                    { 65, 0, 10, "Limonata", 90.0 },
                    { 66, 0, 10, "Çilekli Limonata", 95.0 },
                    { 67, 0, 10, "Naneli Limonata", 95.0 },
                    { 68, 0, 10, "Berry Hibiscus", 100.0 },
                    { 69, 0, 10, "Hibiscus Ice Tea", 90.0 },
                    { 70, 0, 10, "Cool Lime", 110.0 },
                    { 71, 0, 11, "Bluebaby", 130.0 },
                    { 72, 0, 11, "Hermosa", 130.0 },
                    { 73, 0, 11, "Sweet Dreams", 130.0 },
                    { 74, 0, 11, "Summersun", 130.0 },
                    { 75, 0, 11, "Mojito", 130.0 },
                    { 76, 0, 11, "Çilekli Mojito", 130.0 },
                    { 77, 0, 11, "Blueberry Jalep", 130.0 },
                    { 78, 0, 11, "Winterfazz", 130.0 },
                    { 79, 0, 11, "Sunlight", 130.0 },
                    { 80, 0, 12, "Çikolata", 30.0 },
                    { 81, 0, 12, "Karamel", 30.0 },
                    { 82, 0, 12, "Vanilya", 30.0 },
                    { 83, 0, 12, "Fındık", 30.0 },
                    { 84, 0, 12, "Badem", 30.0 },
                    { 85, 0, 12, "Irish", 30.0 },
                    { 86, 0, 12, "Hindistan Cevizi", 30.0 },
                    { 87, 0, 12, "Toffee Nut", 30.0 },
                    { 90, 0, 13, "Pavlova", 180.0 },
                    { 91, 0, 13, "Brownie", 170.0 },
                    { 92, 0, 13, "San Sebastian", 180.0 },
                    { 93, 0, 13, "Paris-Brest", 180.0 },
                    { 94, 0, 13, "Makaron", 120.0 },
                    { 95, 0, 13, "Magnolia", 160.0 },
                    { 96, 0, 13, "Lotus Magnolia", 160.0 },
                    { 97, 0, 13, "Afife", 170.0 },
                    { 98, 0, 13, "Kedi Dili Tiramisu", 170.0 },
                    { 99, 0, 13, "Floransa Cake", 160.0 },
                    { 100, 0, 13, "Orman Meyveli Cheesecake", 155.0 },
                    { 101, 0, 13, "Callia Kruvasan", 190.0 },
                    { 102, 0, 13, "Callia", 200.0 },
                    { 103, 0, 14, "Kruvasan Sandwich", 180.0 },
                    { 104, 0, 14, "Callia Kruvasan", 190.0 },
                    { 105, 0, 9, "Çilek", 100.0 },
                    { 106, 0, 9, "Muz", 100.0 },
                    { 107, 0, 9, "Blueberry", 100.0 },
                    { 108, 0, 9, "Mango Ananas", 100.0 },
                    { 109, 0, 9, "Mango", 100.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 83);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 85);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 86);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 87);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 90);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 91);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 92);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 93);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 94);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 95);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 96);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 97);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 98);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 99);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 100);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 109);
        }
    }
}
