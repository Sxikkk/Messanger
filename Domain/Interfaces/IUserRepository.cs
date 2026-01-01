using Domain.Entities;

namespace Domain.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(Guid userId);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByLoginAsync(string login);
    Task<User?> GetUserByEmailOrLoginAsync(string emailOrLogin);
    Task AddUserAsync(User user);
    Task UpdateUserAsync(User user);
}