using Calia.Services.ProductAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Calia.Services.ProductAPI.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{

		}


		public DbSet<Product> Products { get; set; }
		public DbSet<ProductExtra> ProductExtras { get; set; }
		public DbSet<ProductMaterial> ProductMaterials { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().HasData(
                new Product { ProductId = 1, Name = "Espresso", Price = 70, CategoryId = 1 },
                new Product { ProductId = 2, Name = "Double Espresso", Price = 90, CategoryId = 1 },
                new Product { ProductId = 3, Name = "Espresso Macchiato", Price = 75, CategoryId = 1 },
                new Product { ProductId = 4, Name = "Americano", Price = 95, CategoryId = 1 },
                new Product { ProductId = 5, Name = "Latte", Price = 100, CategoryId = 1 },
                new Product { ProductId = 6, Name = "Flat White", Price = 100, CategoryId = 1 },
                new Product { ProductId = 7, Name = "Cappuccino", Price = 100, CategoryId = 1 },
                new Product { ProductId = 8, Name = "Cortado", Price = 100, CategoryId = 1 },
                new Product { ProductId = 9, Name = "Caramel Macchiato", Price = 110, CategoryId = 1 },
                new Product { ProductId = 10, Name = "Mocha", Price = 100, CategoryId = 1 },
                new Product { ProductId = 11, Name = "White Mocha", Price = 110, CategoryId = 1 },
                new Product { ProductId = 12, Name = "Zebra Mocha", Price = 110, CategoryId = 1 },
                new Product { ProductId = 13, Name = "Lotus Latte", Price = 110, CategoryId = 1 },
                new Product { ProductId = 14, Name = "Pumpkin Spice Latte", Price = 110, CategoryId = 1 },

                new Product { ProductId = 15, Name = "V60", Price = 110, CategoryId = 2 },
                new Product { ProductId = 16, Name = "Chemex", Price = 110, CategoryId = 2 },
                new Product { ProductId = 17, Name = "Filtre Kahve", Price = 90, CategoryId = 2 },
                new Product { ProductId = 18, Name = "Black Eye", Price = 110, CategoryId = 2 },
                new Product { ProductId = 19, Name = "Red Eye", Price = 100, CategoryId = 2, },
                new Product { ProductId = 20, Name = "Death Eye", Price = 120, CategoryId = 2 },

                new Product { ProductId = 21, Name = "Çay", Price = 30, CategoryId = 3 },
                new Product { ProductId = 22, Name = "Fincan Çay", Price = 45, CategoryId = 3 },
                new Product { ProductId = 23, Name = "Sütlü Çay", Price = 65, CategoryId = 3 },
                new Product { ProductId = 24, Name = "Türk Kahvesi", Price = 70, CategoryId = 3 },
                new Product { ProductId = 25, Name = "Double Türk Kahvesi", Price = 90, CategoryId = 3 },
                new Product { ProductId = 26, Name = "Sütlü Menengiç", Price = 75, CategoryId = 3 },
                new Product { ProductId = 27, Name = "Menengiç Kahvesi", Price = 75, CategoryId = 3 },
                new Product { ProductId = 28, Name = "Sıcak Çikolata", Price = 85, CategoryId = 3 },
                new Product { ProductId = 29, Name = "Beyaz Çikolata", Price = 85, CategoryId = 3 },
                new Product { ProductId = 30, Name = "Salep", Price = 85, CategoryId = 3 },
                new Product { ProductId = 31, Name = "Chai Tea Latte", Price = 105, CategoryId = 3 },
                new Product { ProductId = 32, Name = "Ballı Süt", Price = 70, CategoryId = 3 },


                new Product { ProductId = 33, Name = "Su", Price = 30, CategoryId = 4 },
                new Product { ProductId = 34, Name = "Soda", Price = 50, CategoryId = 4 },
                new Product { ProductId = 35, Name = "Meyveli Soda", Price = 55, CategoryId = 4 },
                new Product { ProductId = 36, Name = "Churchill", Price = 60, CategoryId = 4 },
                new Product { ProductId = 37, Name = "Vişne Soda", Price = 70, CategoryId = 4 },
                new Product { ProductId = 38, Name = "Red Bull", Price = 90, CategoryId = 4 },
                new Product { ProductId = 39, Name = "Detox", Price = 100, CategoryId = 4 },


                new Product { ProductId = 40, Name = "Ice Americano", Price = 100, CategoryId = 5 },
                new Product { ProductId = 41, Name = "Ice Filtre Kahve", Price = 100, CategoryId = 5 },
                new Product { ProductId = 42, Name = "Ice Latte", Price = 110, CategoryId = 5 },
                new Product { ProductId = 43, Name = "Ice Mocha", Price = 120, CategoryId = 5 },
                new Product { ProductId = 44, Name = "Ice White Mocha", Price = 120, CategoryId = 5 },
                new Product { ProductId = 45, Name = "Ice Zebra Mocha", Price = 120, CategoryId = 5 },
                new Product { ProductId = 46, Name = "Ice Red Eye", Price = 110, CategoryId = 5 },
                new Product { ProductId = 47, Name = "Ice Black Eye", Price = 105, CategoryId = 5 },
                new Product { ProductId = 48, Name = "Ice Lotus Latte", Price = 120, CategoryId = 5 },
                new Product { ProductId = 49, Name = "Ice Cold Brew Latte", Price = 130, CategoryId = 5 },
                new Product { ProductId = 50, Name = "Frappe", Price = 120, CategoryId = 5 },
                new Product { ProductId = 51, Name = "Affagato", Price = 120, CategoryId = 5 },
                new Product { ProductId = 52, Name = "Freddo Cappuccino", Price = 130, CategoryId = 5 },
                new Product { ProductId = 53, Name = "Cold Brew", Price = 120, CategoryId = 5 },


                new Product { ProductId = 54, Name = "Ihlamur", Price = 80, CategoryId = 6 },
                new Product { ProductId = 55, Name = "Yeşil Çay", Price = 80, CategoryId = 6 },
                new Product { ProductId = 56, Name = "Yaseminli Yeşil Çay", Price = 80, CategoryId = 6 },
                new Product { ProductId = 57, Name = "Kış Çayı", Price = 80, CategoryId = 6 },
                new Product { ProductId = 58, Name = "Ada Çayı", Price = 80, CategoryId = 6 },
                new Product { ProductId = 59, Name = "Hibiscus", Price = 80, CategoryId = 6 },


                new Product { ProductId = 60, Name = "Çilek", Price = 100, CategoryId = 7 },
                new Product { ProductId = 61, Name = "Blueberry", Price = 100, CategoryId = 7 },
                new Product { ProductId = 62, Name = "Mango", Price = 100, CategoryId = 7 },



                new Product { ProductId = 65, Name = "Limonata", Price = 90, CategoryId = 10 },
                new Product { ProductId = 66, Name = "Çilekli Limonata", Price = 95, CategoryId = 10 },
                new Product { ProductId = 67, Name = "Naneli Limonata", Price = 95, CategoryId = 10 },
                new Product { ProductId = 68, Name = "Berry Hibiscus", Price = 100, CategoryId = 10 },
                new Product { ProductId = 69, Name = "Hibiscus Ice Tea", Price = 90, CategoryId = 10 },
                new Product { ProductId = 70, Name = "Cool Lime", Price = 110, CategoryId = 10 },


                new Product { ProductId = 71, Name = "Bluebaby", Price = 130, CategoryId = 11 },
                new Product { ProductId = 72, Name = "Hermosa", Price = 130, CategoryId = 11 },
                new Product { ProductId = 73, Name = "Sweet Dreams", Price = 130, CategoryId = 11 },
                new Product { ProductId = 74, Name = "Summersun", Price = 130, CategoryId = 11 },
                new Product { ProductId = 75, Name = "Mojito", Price = 130, CategoryId = 11 },
                new Product { ProductId = 76, Name = "Çilekli Mojito", Price = 130, CategoryId = 11 },
                new Product { ProductId = 77, Name = "Blueberry Jalep", Price = 130, CategoryId = 11 },
                new Product { ProductId = 78, Name = "Winterfazz", Price = 130, CategoryId = 11 },
                new Product { ProductId = 79, Name = "Sunlight", Price = 130, CategoryId = 11 },


                new Product { ProductId = 80, Name = "Çikolata", Price = 30, CategoryId = 12 },
                new Product { ProductId = 81, Name = "Karamel", Price = 30, CategoryId = 12 },
                new Product { ProductId = 82, Name = "Vanilya", Price = 30, CategoryId = 12 },
                new Product { ProductId = 83, Name = "Fındık", Price = 30, CategoryId = 12 },
                new Product { ProductId = 84, Name = "Badem", Price = 30, CategoryId = 12 },
                new Product { ProductId = 85, Name = "Irish", Price = 30, CategoryId = 12 },
                new Product { ProductId = 86, Name = "Hindistan Cevizi", Price = 30, CategoryId = 12 },
                new Product { ProductId = 87, Name = "Toffee Nut", Price = 30, CategoryId = 12 },




                new Product { ProductId = 90, Name = "Pavlova", Price = 180, CategoryId = 13,AvailableProducts=20 },
                new Product { ProductId = 91, Name = "Brownie", Price = 170, CategoryId = 13, AvailableProducts = 20 },
                new Product { ProductId = 92, Name = "San Sebastian", Price = 180, CategoryId = 13, AvailableProducts = 20 },
                new Product { ProductId = 93, Name = "Paris-Brest", Price = 180, CategoryId = 13, AvailableProducts = 20 },
                new Product { ProductId = 94, Name = "Makaron", Price = 120, CategoryId = 13, AvailableProducts = 20 },
                new Product { ProductId = 95, Name = "Magnolia", Price = 160, CategoryId = 13, AvailableProducts = 20 },
                new Product { ProductId = 96, Name = "Lotus Magnolia", Price = 160, CategoryId = 13 , AvailableProducts = 20 },
                new Product { ProductId = 97, Name = "Afife", Price = 170, CategoryId = 13 , AvailableProducts = 20 },
                new Product { ProductId = 98, Name = "Kedi Dili Tiramisu", Price = 170, CategoryId = 13 , AvailableProducts = 20 },
                new Product { ProductId = 99, Name = "Floransa Cake", Price = 160, CategoryId = 13 , AvailableProducts = 20 },
                new Product { ProductId = 100, Name = "Orman Meyveli Cheesecake", Price = 155, CategoryId = 13 , AvailableProducts = 20 },
                new Product { ProductId = 101, Name = "Callia Kruvasan", Price = 190, CategoryId = 13 , AvailableProducts = 20 },
                new Product { ProductId = 102, Name = "Callia", Price = 200, CategoryId = 13 , AvailableProducts = 20 },
                new Product { ProductId = 103, Name = "Kruvasan Sandwich", Price = 180, CategoryId = 14, AvailableProducts = 20 },
                new Product { ProductId = 104, Name = "Callia Kruvasan", Price = 190, CategoryId = 14 , AvailableProducts = 20 },


                new Product { ProductId = 105, Name = "Çilek", Price = 100, CategoryId = 9 },
                new Product { ProductId = 106, Name = "Muz", Price = 100, CategoryId = 9 },
                new Product { ProductId = 107, Name = "Blueberry", Price = 100, CategoryId = 9 },
                new Product { ProductId = 108, Name = "Mango Ananas", Price = 100, CategoryId = 9 },
                new Product { ProductId = 109, Name = "Mango", Price = 100, CategoryId = 9 }

            );
        }
	}
}
