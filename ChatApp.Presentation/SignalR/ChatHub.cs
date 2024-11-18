using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Presentation.SignalR
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string chatRoomId, string senderId, string content)
        {
            await Clients.Group(chatRoomId).SendAsync("ReceiveMessage", senderId, content);
        }

        public async Task JoinChatRoom(string chatRoomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatRoomId);
        }

        public async Task LeaveChatRoom(string chatRoomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatRoomId);
        }
    }

}
