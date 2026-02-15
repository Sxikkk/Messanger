namespace Application.Interfaces.CacheInterfaces;

public interface ISessionCache
{
    Task SetSessionAsync(Guid sessionId);
    Task RemoveSessionAsync(Guid sessionId);
    Task<bool> HasSessionAsync(Guid sessionId);
}