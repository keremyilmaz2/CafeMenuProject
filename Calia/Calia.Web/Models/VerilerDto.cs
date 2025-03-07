using System.ComponentModel.DataAnnotations;

namespace Calia.Web.Models
{
    public class VerilerDto
    {
        [Key]
        public int Id { get; set; }
        public int TotalProductNumber { get; set; }
        public int ToplamKazanç { get; set; }
        public int ToplamSiparisSayisi { get; set; }
        public int ToplamKapananMasaSayisi { get; set; }
        public List<double> HaftalikNakit { get; set; }
        public List<double> HaftalikKrediKarti { get; set; }
        public List<double> AylikNakit { get; set; }
        public List<double> AylikKrediKarti { get; set; }
        public DateTime SonGuncellemeTarihi { get; set; }
    }
}
