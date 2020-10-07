using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Storage.API.Data;

namespace Storage.API_CAN.SignalR
{

    public class PresenceHub : Hub
    {
       
        public override async Task OnConnectedAsync()
        {
            await Clients.Others.SendAsync("UserIsOnline");
        }

        public async Task OnDisconnectedAsync()
        {
            await Clients.Others.SendAsync("UserIsOffline");
        }
    }
}