using System.ComponentModel.DataAnnotations;

namespace Calia.Services.OrderAPI.Models
{
    public class Veriler
    {
        [Key]
        public int Id { get; set; }
        public int TotalProductNumber { get; set; } = 0;
        public int ToplamKazanç { get; set; } = 0;
        public int ToplamSiparisSayisi { get; set; } = 0;
        public int ToplamKapananMasaSayisi { get; set; } = 0;
        public List<double>? HaftalikNakit { get; set; } = new List<double> { 0, 0, 0, 0, 0, 0, 0 };
        public List<double>? HaftalikKrediKarti { get; set; } = new List<double> { 0, 0, 0, 0, 0, 0, 0 };
        public List<double>? AylikNakit { get; set; } = new List<double> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public List<double>? AylikKrediKarti { get; set; } = new List<double> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public DateTime SonGuncellemeTarihi { get; set; } = DateTime.Now;
        public DateTime SonGuncellemeTarihiYil { get; set; } = DateTime.Now;
    }
}
