using Calia.Services.StockAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Calia.Services.StockAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { 
        
        }
		public DbSet<StockMaterial> StockMaterials { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
			

		}

	}
}
