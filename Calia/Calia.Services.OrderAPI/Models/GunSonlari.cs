using System.ComponentModel.DataAnnotations;

namespace Calia.Services.OrderAPI.Models
{
    public class GunSonlari
    {
        [Key]
        public int GunSonlariId { get; set; }
        public double GunlukNakit { get; set; }
        public double GunlukKrediKarti { get; set; }
        public double GunlukIskonto { get; set; }
        public double GunlukIkram { get; set; }
        public double GunlukGider { get; set; }
        public DateTime GununTarih { get; set; }
    }
}
