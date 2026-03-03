namespace Application.Interfaces.CacheInterfaces;

public interface IOnlineCache
{
    Task SetOnlineAsync(string username);
    Task SetOfflineAsync(string username);
    Task<bool> IsOnlineAsync(string username);
}