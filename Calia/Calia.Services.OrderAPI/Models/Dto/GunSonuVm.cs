namespace Calia.Services.OrderAPI.Models.Dto
{
    public class GunSonuVm
    {
        public GunSonuDto GunSonu { get; set; }
        public Dictionary<string, double> MasalarınToplamKazancı { get; set; }
        public List<OrderDetailsDto> EnCokSatilanUrunler { get; set; } // En çok satılan ürünler
        public List<CancelDetailsDto> IptalEdilenSiparisler { get; set; } // İptal edilen siparişler
        public Dictionary<string, double> GelirGiderRaporu { get; set; } // Günlük gelir-gider raporu
    }
}
