using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Presentation.SignalR
{
    public class WebRTCHub : Hub
    {
        public async Task SendSignal(string receiverId, string signalData)
        {
            await Clients.User(receiverId).SendAsync("ReceiveSignal", Context.ConnectionId, signalData);
        }

        public async Task JoinCall(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }
    }
}
