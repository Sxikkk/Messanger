
using System.Text.Json;
using Application.Interfaces;
using StackExchange.Redis;

namespace Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _redisDb;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _redisDb = redis.GetDatabase();
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan ttl)
    {
        var json = JsonSerializer.Serialize(value);

        RedisKey redisKey = key;
        RedisValue redisValue = json;

        await _redisDb.StringSetAsync(redisKey, redisValue, ttl);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        RedisKey redisKey = key;
        var redisValue = await _redisDb.StringGetAsync(redisKey);

        return !redisValue.HasValue ? default : JsonSerializer.Deserialize<T>(redisValue!);
    }

    public async Task RemoveAsync(string key)
    {
        RedisKey redisKey = key;
        await _redisDb.KeyDeleteAsync(redisKey);
    }
}