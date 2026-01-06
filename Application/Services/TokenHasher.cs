using System.Security.Cryptography;
using System.Text;
using Application.Interfaces;

namespace Application.Services;

public class TokenHasher : ITokenHasher
{
    private readonly byte[] _secret;

    public TokenHasher(IConfiguration configuration)
    {
        var key = configuration["Jwt:Key"] 
                  ?? throw new InvalidOperationException("JWT Key missing for TokenHasher");
        _secret = Encoding.UTF8.GetBytes(key);
    }
    public string HashToken(string token)
    {
        using var hmac = new HMACSHA256(_secret);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hash);
    }

    public bool VerifyToken(string token, string? hashedToken)
    {
        if (hashedToken is null) return false;
        var computed = HashToken(token);
        return computed == hashedToken;
    }
}