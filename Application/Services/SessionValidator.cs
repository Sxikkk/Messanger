using Application.Interfaces;
using Domain.Interfaces;

namespace Application.Services;

public class SessionValidator : ISessionValidator
{
    private readonly IUserSessionRepository _userSessionRepository;

    public SessionValidator(IUserSessionRepository userSessionRepository)
    {
        _userSessionRepository = userSessionRepository;
    }

    public async Task<bool> IsSessionValidAsync(string userId, string deviceId)
    {
        return await _userSessionRepository.IsExist(Guid.Parse(userId), deviceId);
    }
}