namespace Calia.Web.Models
{
    public class GunSonuDto
    {
        public string? GununAdi { get; set; }
        public double? ToplamKazanç { get; set; }
        public double? SatilanUrunFiyati { get; set; }
        public int? ToplamSiparisSayisi { get; set; }
        public double? GunlukNakit { get; set; }
        public double? GunlukKrediKarti { get; set; }
        public double? GunlukIskonto { get; set; }
        public double? GunlukIkram { get; set; }
        public double? GunlukGider { get; set; }
        public DateTime? GununTarih { get; set; }
    }
}
