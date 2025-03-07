namespace Calia.Services.OrderAPI.Service.IService
{
    public interface IPrintNodeService
    {
        Task<string> GetPrintersAsync();
        Task<string> PrintFileAsync(string printerId, string filePath);
    }
}
