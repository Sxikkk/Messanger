using Application.Interfaces.CacheInterfaces;
using StackExchange.Redis;

namespace Infrastructure.Services.Redis;

public class RedisSessionStore : ISessionCache
{
    private readonly IDatabase _db;
    private readonly TimeSpan _ttl;

    private const string Prefix = "session:";

    public RedisSessionStore(
        IConnectionMultiplexer redis,
        IConfiguration configuration)
    {
        _db = redis.GetDatabase();
        _ttl = TimeSpan.FromMinutes(
            configuration.GetValue<int>("Jwt:AccessTokenExpiryMinutes"));
    }

    public async Task SetSessionAsync(Guid sessionId)
    {
        await _db.StringSetAsync(
            Prefix + sessionId,
            "1",
            _ttl);
    }

    public async Task RemoveSessionAsync(Guid sessionId)
    {
        await _db.KeyDeleteAsync(Prefix + sessionId);
    }

    public async Task<bool> HasSessionAsync(Guid sessionId)
    {
        return await _db.KeyExistsAsync(Prefix + sessionId);
    }
}