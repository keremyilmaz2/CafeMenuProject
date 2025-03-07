using Microsoft.AspNetCore.SignalR;

namespace Calia.Services.ShoppingCartAPI.Hubs
{
    public class CartHub :Hub
    {
        public async Task SendTableStatusUpdate(int count)
        {
            await Clients.All.SendAsync("ReceiveTableStatusUpdate", count);
        }
    }
}
