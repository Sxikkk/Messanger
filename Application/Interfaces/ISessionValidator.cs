namespace Application.Interfaces;

public interface ISessionValidator
{
    Task<bool> IsSessionValidAsync(string userId, string deviceId);
}