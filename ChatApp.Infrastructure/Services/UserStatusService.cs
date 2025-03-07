using ChatApp.Application.Abstracts.Services;
using StackExchange.Redis;

namespace ChatApp.Infrastructure.Services
{
    public class UserStatusService : IUserStatusService
    {
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IDatabase _redisDatabase;
        public UserStatusService(IConnectionMultiplexer redisConnection)
        {
            _redisConnection = redisConnection;
            _redisDatabase = redisConnection.GetDatabase();
        }
        //public async Task SetUserOnline(string userId)
        //{
        //    //var key = $"user:{userId}:status";
        //    //await _redisDatabase.StringSetAsync(key, "Online", TimeSpan.FromMinutes(30));

        //}
        public async Task SetUserOnline(string userId)
        {
            await _redisDatabase.StringSetAsync($"user:{userId}:status", "Online", TimeSpan.FromHours(1));
        }

        public async Task SetUserOffline(string userId)
        {
            var key = $"user:{userId}:status";
            await _redisDatabase.KeyDeleteAsync(key);
        }
        public async Task<bool> IsUserOnline(string userId)
        {
            var key = $"user:{userId}:status";
            var status = await _redisDatabase.StringGetAsync(key);
            return status == "Online";
        }
        public async Task<List<string>> GetOnlineUsers()
        {
            var keys = _redisConnection.GetServer(_redisConnection.GetEndPoints().First())
                        .Keys(pattern: "user:*:status")
                        .ToArray();

            var users = new List<string>();

            foreach (var key in keys)
            {
                var status = await _redisDatabase.StringGetAsync(key);
                Console.WriteLine($"🟢 Checking {key}: {status}");

                if (status == "Online")
                {
                    users.Add(key.ToString().Replace("user:", "").Replace(":status", ""));
                }
            }
            return users;
        }









    }
}
