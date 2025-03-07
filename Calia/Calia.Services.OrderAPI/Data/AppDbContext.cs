using Calia.Services.OrderAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Calia.Services.OrderAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { 
        
        }


        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }

        public DbSet<OrderExtra> OrderExtras { get; set; }
        public DbSet<TableNo> TableNos { get; set; }
        public DbSet<TableDetails> tableDetails { get; set; }
        public DbSet<KisiselGider> KisiselGiders { get; set; }
        public DbSet<AdminNames> AdminNames { get; set; }
        public DbSet<Veriler> Veriler { get; set; }
        public DbSet<CancelDetails> CancelDetails { get; set; }
        public DbSet<GunSonlari> GunSonlaris { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TableNo>().HasData(
                new TableNo { Id = 1, MasaNo = "1" },
                new TableNo { Id = 2, MasaNo = "2" },
                new TableNo { Id = 3, MasaNo = "3" },
                new TableNo { Id = 4, MasaNo = "4" },
                new TableNo { Id = 5, MasaNo = "5" },
                new TableNo { Id = 6, MasaNo = "6" },
                new TableNo { Id = 7, MasaNo = "7" },
                new TableNo { Id = 8, MasaNo = "8" },
                new TableNo { Id = 9, MasaNo = "9" },
                new TableNo { Id = 10, MasaNo = "10" },
                new TableNo { Id = 11, MasaNo = "11" },
                new TableNo { Id = 12, MasaNo = "12" },
                new TableNo { Id = 13, MasaNo = "13" },
                new TableNo { Id = 14, MasaNo = "14" },
                new TableNo { Id = 15, MasaNo = "15" },
                new TableNo { Id = 16, MasaNo = "16" },
                new TableNo { Id = 17, MasaNo = "17" },
                new TableNo { Id = 18, MasaNo = "18" },
                new TableNo { Id = 19, MasaNo = "19" },
                new TableNo { Id = 20, MasaNo = "20" },
                new TableNo { Id = 21, MasaNo = "21" },
                new TableNo { Id = 22, MasaNo = "22" },
                new TableNo { Id = 23, MasaNo = "23" },
                new TableNo { Id = 24, MasaNo = "24" },
                new TableNo { Id = 25, MasaNo = "25" },
                new TableNo { Id = 26, MasaNo = "26" },
                new TableNo { Id = 27, MasaNo = "27" },
                new TableNo { Id = 28, MasaNo = "28" },
                new TableNo { Id = 29, MasaNo = "29" },
                new TableNo { Id = 30, MasaNo = "30" },
                new TableNo { Id = 31, MasaNo = "31" }
            );


            modelBuilder.Entity<AdminNames>().HasData(
                new AdminNames { Id = 1, AdminName = "Hasan" },
                new AdminNames { Id = 2, AdminName = "Berke" },
                new AdminNames { Id = 3, AdminName = "Mehmet" },
                new AdminNames { Id = 4, AdminName = "Samet" }
            );
        }


    }
}
