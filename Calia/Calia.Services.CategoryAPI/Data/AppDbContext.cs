using Calia.Services.CategoryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Calia.Services.CategoryAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { 
        
        }


        public DbSet<Category> Categories { get; set; }
		public DbSet<CategoryMaterial> CategoryMaterials  { get; set; }

		public DbSet<CategoryExtra> CategoryExtras { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<Category>().HasData(
                 new Category { Id = 1, Name = "Espresso Bar", DisplayOrder = 1, ProductCount = false },
                 new Category { Id = 2, Name = "Brew Bar", DisplayOrder = 2, ProductCount = false },
                 new Category { Id = 3, Name = "Hot Drinks", DisplayOrder = 3, ProductCount = false },
                 new Category { Id = 4, Name = "Soft Drinks", DisplayOrder = 4, ProductCount = false },
                 new Category { Id = 5, Name = "Ice Coffee", DisplayOrder = 5, ProductCount = false },
                 new Category { Id = 6, Name = "Bitki Çayları", DisplayOrder = 6, ProductCount = false },
                 new Category { Id = 7, Name = "Frozen", DisplayOrder = 7, ProductCount = false },
                 new Category { Id = 8, Name = "Milkshake", DisplayOrder = 8, ProductCount = false },
                 new Category { Id = 9, Name = "Smoothie", DisplayOrder = 9, ProductCount = false },
                 new Category { Id = 10, Name = "Handmade", DisplayOrder = 10, ProductCount = false },
                 new Category { Id = 11, Name = "Moctails", DisplayOrder = 11, ProductCount = false },
                 new Category { Id = 12, Name = "Şuruplar", DisplayOrder = 12, ProductCount = false },
                 new Category { Id = 13, Name = "Tatlılar", DisplayOrder = 13, ProductCount = true },
                 new Category { Id = 14, Name = "Kruvasan", DisplayOrder = 14, ProductCount = true }
            );

		}

	}
}
