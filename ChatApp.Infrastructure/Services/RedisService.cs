﻿using ChatApp.Application.Abstracts.Services;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

namespace ChatApp.Infrastructure.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        public RedisService(IDistributedCache distributedCache, IConnectionMultiplexer connectionMultiplexer)
        {
            _distributedCache = distributedCache;
            _connectionMultiplexer = connectionMultiplexer;
        }
        public async Task<List<T>?> GetDataByEndpoint<T>(string endpoint)
        {
            var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First());
            var keys = server.Keys(pattern: endpoint + "*").ToArray();
            var result = new List<T>();

            var db = _connectionMultiplexer.GetDatabase();
            foreach (var key in keys)
            {
                var data = await db.StringGetAsync(key);
                if (!data.IsNullOrEmpty)
                {
                    var deserializedData = JsonConvert.DeserializeObject<T>(data);
                    result.Add(deserializedData);
                }
            }
            return result;

        }

        public async Task<T> GetDataObjectByKey<T>(string key)
        {
            var objectValue = await _distributedCache.GetStringAsync(key);
            if (string.IsNullOrEmpty(objectValue))
            {
                return JsonConvert.DeserializeObject<T>(objectValue);
            }
            return default(T);

        }

        public async Task RemoveDataByKey(string key)
        {
            var res = await _distributedCache.GetStringAsync(key);
            if (!string.IsNullOrEmpty(res))
            {
                await _distributedCache.RemoveAsync(key);
            }

        }

        public async Task SetData(string key, object data, TimeSpan? time)
        {
            if (data != null)
            {
                var serializerResponse = JsonConvert.SerializeObject(data, new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
                await _distributedCache.SetStringAsync(key, serializerResponse, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = time
                });
            }
        }

        public async Task UploadDataByKey(string key, object data)
        {
            var db = _connectionMultiplexer.GetDatabase();
            var serializerResponse = JsonConvert.SerializeObject(data, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            bool isSuccess = await db.StringSetAsync(key, serializerResponse);
            if (!isSuccess)
            {
                throw new Exception("Failed to upload data to redis");
            }
        }
    }
}
