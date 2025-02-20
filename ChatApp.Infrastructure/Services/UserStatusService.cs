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
        public async Task SetUserOnline(string userId)
        {
            var key = $"user:{userId}:status";
            await _redisDatabase.StringSetAsync(key, "Online", TimeSpan.FromMinutes(30));
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
        public async Task<int> GetOnlineUsersCount()
        {
            var server = _redisConnection.GetServer(_redisConnection.GetEndPoints().First());
            var keys = server.Keys(pattern: "user:*:status").ToArray();
            int count = 0;

            foreach (var key in keys)
            {
                var status = await _redisDatabase.StringGetAsync(key);
                if (status == "Online")
                {
                    count++;
                }
            }
            return count;
        }

        

        

        
    }
}
