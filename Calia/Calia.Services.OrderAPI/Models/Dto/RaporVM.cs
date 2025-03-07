namespace Calia.Services.OrderAPI.Models.Dto
{
    public class RaporVM
    {
        public VerilerDto Veriler { get; set; }
        public List<TableDetailsDto> MasalarınGünlükRaporu { get; set; }
        public Dictionary<string, double> MasalarınToplamKazancı { get; set; }
        public List<CancelDetailsDto>? İptaller { get; set; }
        public Dictionary<string, double> GünlükGelirGiderRaporu { get; set; }
        public Dictionary<string, double> GünlükGarsonRaporu { get; set; }
        public List<VerilenIkramlarDto> IkramVerilenUrunler { get; set; }
        public IEnumerable<StockMaterialDto> Malzemeler { get; set; }
        public List<OrderDetailsDto> SatilanUrunler { get; set; }
        public List<ProductDto> KalanTatlılar { get; set; }
        public List<SatilanUrunlerDto> UrunBasinaSatilma { get; set; }

    }
}
