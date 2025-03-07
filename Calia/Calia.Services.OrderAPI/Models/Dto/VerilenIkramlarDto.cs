namespace Calia.Services.OrderAPI.Models.Dto
{
    public class VerilenIkramlarDto
    {
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public double Price { get; set; }
        public int IkramAdedi { get; set; }
    }
}