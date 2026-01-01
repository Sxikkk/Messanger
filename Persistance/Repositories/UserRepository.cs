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

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _dbContext.Users.ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }
    
    public async Task<User?> GetUserByEmailOrLoginAsync(string emailOrLogin)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == emailOrLogin || u.Login == emailOrLogin);
    }

    public async Task<User?> GetUserByLoginAsync(string login)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Login == login);
    }

    public async Task AddUserAsync(User user)
    {
        var exists = await _dbContext.Users.AnyAsync(u =>
            u.Email == user.Email || u.Login == user.Login);

        if (exists)
            throw new AlreadyExistException($"{user.Login} - {user.Email}");

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        try
        {
            _dbContext.Users.Update(user);
        }
        finally
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}