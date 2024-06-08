using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace NUS_Orbital.Hubs
{
    public class ChatHub : Hub
    {
        // Method to send messages to all clients
        public async Task SendMessage(string user, string message)
        {
            // Call the "ReceiveMessage" method on all connected clients
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}