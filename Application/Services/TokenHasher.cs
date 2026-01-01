using Application.Interfaces;

namespace Application.Services;

public class TokenHasher: ITokenHasher
{
    private const int WorkFactor = 12;

    public string? HashToken(string token)
    {
        return BCrypt.Net.BCrypt.HashPassword(token, WorkFactor);
    }

    public bool VerifyToken(string token, string? hashedToken)
    {
        return BCrypt.Net.BCrypt.Verify(token, hashedToken);
    }}