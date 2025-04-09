using ChatApp.Application.Abstracts.Services;
using ChatApp.Application.DTOs.Message;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace ChatApp.Presentation.SignalR
{

    public class ChatHub : Hub
    {
        private readonly IUserStatusService _userStatusService;

        private static ConcurrentDictionary<string, string> OnlineUsers = new();

        public ChatHub(IUserStatusService userStatusService)
        {
            _userStatusService = userStatusService;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            OnlineUsers[Context.ConnectionId] = userId;
            await _userStatusService.SetUserOnline(userId);
            await Clients.All.SendAsync("UserOnline", userId);

            await base.OnConnectedAsync();
        }



        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (OnlineUsers.TryRemove(Context.ConnectionId, out var userId))
            {
                await _userStatusService.SetUserOffline(userId);
                await Clients.All.SendAsync("UserOffline", userId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendPrivateMessage(MessageAddDto message)
        {
            await Clients.User(message.RecipientId!).SendAsync("ReceiveMessage", message);
 
        }

        public async Task SendGroupMessage(string groupId, string content)
        {
            await Clients.Group(groupId).SendAsync("ReceiveMessage", content);
            await Clients.Group(groupId).SendAsync("NewMessageNotification", groupId);
        }

        public async Task JoinChatRoom(string groupId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
        }

        public async Task LeaveChatRoom(string groupId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
        }

        public async Task GetOnlineUsers()
        {
            var onlineUsers = await _userStatusService.GetOnlineUsers();
            await Clients.Caller.SendAsync("ReceiveOnlineUsers", onlineUsers);
        }

        public async Task ReceiveNotification(string recipientId, string content)
        {
            await Clients.User(recipientId).SendAsync("ReceiveNotification", content);
        }

    }

}
