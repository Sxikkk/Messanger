using System.Text.Json;
using Application.Interfaces.CacheInterfaces;
using Domain.ValueObjects;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Infrastructure.Services.Redis;

public class RedisOnlineStore: IOnlineCache
{
    private readonly IDatabase _db;
    private readonly TimeSpan _ttl;

    private const string Prefix = "online:";
    
    public RedisOnlineStore(
        IConnectionMultiplexer redis,
        IOptions<OnlineStatusSettings> onlineSettings)
    {
        _db = redis.GetDatabase();
        _ttl = TimeSpan.FromMinutes(onlineSettings.Value.HeartbeatMinutes);
    }


    public async Task SetOnlineAsync(string username)
    {
        var key = Prefix + username;

        OnlineStatus? entry;

        var value = await _db.StringGetAsync(key);
        if (value.HasValue)
        {
            entry = JsonSerializer.Deserialize<OnlineStatus>(value!);
            entry?.SetOnline();
        }
        else
        {
            entry = new OnlineStatus(EOnlineStatus.Online);
        }

        await _db.StringSetAsync(key, JsonSerializer.Serialize(entry), _ttl);
    }


    public async Task SetOfflineAsync(string username)
    {
        var key = Prefix + username;
        if (_db.KeyExists(key))
        {
            var outEntry = JsonSerializer.Deserialize<OnlineStatus>((await _db.StringGetAsync(key))!);
            outEntry?.SetOffline();
        }
        else
        {
            var entry = new OnlineStatus(EOnlineStatus.Offline);
            await _db.StringSetAsync(key, JsonSerializer.Serialize(entry), _ttl);
        }
    }

    public async Task<bool> IsOnlineAsync(string username)
    {
        var key = Prefix + username;
        var value = await _db.StringGetAsync(key);
        if (!value.HasValue) return false;

        var status = JsonSerializer.Deserialize<OnlineStatus>(value!);
        return status?.Status == EOnlineStatus.Online;
    }
}