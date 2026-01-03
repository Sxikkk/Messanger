using Domain.Entities;

namespace Domain.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken);
    Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
    Task<User?> GetUserByLoginAsync(string login, CancellationToken cancellationToken);
    Task<User?> GetUserByEmailOrLoginAsync(string emailOrLogin, CancellationToken cancellationToken);
    Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken);
    Task AddUserAsync(User user, CancellationToken cancellationToken);
    Task UpdateUserAsync(User user, CancellationToken cancellationToken);
    Task<Guid> GetUserIdByUsernameAsync(string username, CancellationToken cancellationToken);
    Task<Dictionary<Guid, string>> GetUsernamesByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct);
}