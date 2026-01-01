using Application.Interfaces;
using Microsoft.Win32.SafeHandles;

namespace Application.Services;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);

    public bool VerifyHashedPassword(string userPassword, string requestPassword) => BCrypt.Net.BCrypt.Verify(requestPassword, userPassword);
}