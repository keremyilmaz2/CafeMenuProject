using Calia.Services.OrderAPI.Models.Dto;
using Microsoft.AspNetCore.SignalR;

namespace Calia.Services.OrderAPI.Hubs
{
    public class TableHub : Hub
    {
        public async Task SendTableStatusUpdate(IEnumerable<TableNoDto> tables)
        {
            await Clients.All.SendAsync("ReceiveTableStatusUpdate", tables);
        }
    }
}
