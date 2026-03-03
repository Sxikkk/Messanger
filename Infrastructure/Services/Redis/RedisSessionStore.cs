using Application;
using Application.Interfaces.CacheInterfaces;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Infrastructure.Services.Redis;

public class RedisSessionStore : ISessionCache
{
    private readonly IDatabase _db;
    private readonly TimeSpan _ttl;

    private const string Prefix = "session:";

    public RedisSessionStore(
        IConnectionMultiplexer redis,
        IOptions<JwtSettings> jwtSettings)
    {
        _db = redis.GetDatabase();
        _ttl = TimeSpan.FromMinutes(jwtSettings.Value.AccessTokenExpiryMinutes);
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