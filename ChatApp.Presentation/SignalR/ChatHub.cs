using ChatApp.Application.Abstracts.Services;
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
                Console.WriteLine("❌ Lỗi: UserIdentifier is NULL! Kiểm tra authentication.");
                return;
            }

            OnlineUsers[Context.ConnectionId] = userId;
            await _userStatusService.SetUserOnline(userId);
            Console.WriteLine($"🔵 Thêm User Online: {userId} với ConnectionId: {Context.ConnectionId}");

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

        public async Task SendPrivateMessage(string userId, string content)
        {
            await Clients.User(userId).SendAsync("ReceiveMessage", content);
            await Clients.User(userId).SendAsync("NewMessageNotification", userId);
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
            Console.WriteLine("📢 Sending online users: " + string.Join(", ", onlineUsers));
            await Clients.Caller.SendAsync("ReceiveOnlineUsers", onlineUsers);
        }

    }

}
