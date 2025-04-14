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

        private static ConcurrentDictionary<string, HashSet<string>> OnlineUsers = new();

        public ChatHub(IUserStatusService userStatusService)
        {
            _userStatusService = userStatusService;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier; // Lấy userId từ UserIdProvider
            Console.WriteLine($"[Connected] userId: {userId}, connectionId: {Context.ConnectionId}");

            if (string.IsNullOrEmpty(userId))
                return;

            // Thêm connectionId vào danh sách của userId
            OnlineUsers.AddOrUpdate(userId,
                new HashSet<string> { Context.ConnectionId },
                (key, oldSet) =>
                {
                    lock (oldSet) { oldSet.Add(Context.ConnectionId); }
                    return oldSet;
                });

            await _userStatusService.SetUserOnline(userId);
            await Clients.All.SendAsync("UserOnline", userId);

            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId))
                return;

            // Delay nhỏ để đợi các kết nối khác nếu là reload
            await Task.Delay(1000);

            if (OnlineUsers.TryGetValue(userId, out var connections))
            {
                lock (connections)
                {
                    connections.Remove(Context.ConnectionId);
                    if (connections.Count == 0)
                    {
                        OnlineUsers.TryRemove(userId, out _);
                    }
                }

                if (!OnlineUsers.ContainsKey(userId))
                {
                    await _userStatusService.SetUserOffline(userId);
                    await Clients.All.SendAsync("UserOffline", userId);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }


        public async Task SendPrivateMessage(MessageAddDto message)
        {
            if (OnlineUsers.TryGetValue(message.RecipientId!, out var connections))
            {
                var firstConnectionId = connections.FirstOrDefault();
                if (firstConnectionId != null)
                {
                    await Clients.Client(firstConnectionId).SendAsync("ReceiveMessage", message);
                }
            }

        }

        public async Task SendGroupMessage(MessageAddDto message)
        {
            await Clients.Group(message.GroupId.ToString()!).SendAsync("ReceiveMessage", message);
        }

        public async Task JoinGroup(int groupId)
        {
            var connectionId = Context.ConnectionId;
            await Groups.AddToGroupAsync(connectionId, groupId.ToString());
            Console.WriteLine($"✅ Connection {connectionId} joined group {groupId}");
        }


        public async Task LeaveChatRoom(string groupId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
        }

        public async Task GetOnlineUsers()
        {
            var onlineUserIds = OnlineUsers.Keys.ToList();
            await Clients.Caller.SendAsync("ReceiveOnlineUsers", onlineUserIds);
        }

        public async Task ReceiveNotification(string recipientId, string content)
        {
            await Clients.User(recipientId).SendAsync("ReceiveNotification", content);
        }

    }

}
