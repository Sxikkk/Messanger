using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Persistance.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MyAppDbContext _dbContext;

    public UserRepository(MyAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Users.ToListAsync(cancellationToken);
    }

    public async Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .Include(u => u.Settings)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken: cancellationToken);
    }

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .Include(u => u.Settings)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken: cancellationToken);
    }

    public async Task<User?> GetUserByEmailOrLoginAsync(string emailOrLogin, CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .Include(u => u.Settings)
            .FirstOrDefaultAsync(u => u.Email == emailOrLogin || u.Login == emailOrLogin,
                cancellationToken: cancellationToken);
    }

    public async Task<User?> GetUserByLoginAsync(string login, CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .Include(u => u.Settings)
            .FirstOrDefaultAsync(u => u.Login == login, cancellationToken: cancellationToken);
    }

    public async Task AddUserAsync(User user, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.Users.AnyAsync(u =>
            u.Email == user.Email || u.Login == user.Login, cancellationToken: cancellationToken);

        if (exists)
            throw new AlreadyExistException($"{user.Login} - {user.Email}");

        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateUserAsync(User user, CancellationToken cancellationToken)
    {
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .Include(u => u.Settings)
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken: cancellationToken);
    }
}