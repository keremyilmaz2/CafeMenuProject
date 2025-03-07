namespace Calia.Web.Models
{
    public class KisiselGiderDto
    {
        public int Id { get; set; }
        public int? AdminId { get; set; }
        public AdminNamesDto? AdminName { get; set; }
        public double? AlinanPara { get; set; }

        public string Reason { get; set; } // Sebep eklendi
    }
}
