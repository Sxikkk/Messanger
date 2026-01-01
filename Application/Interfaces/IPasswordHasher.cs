namespace Application.Interfaces;

public interface IPasswordHasher
{
    public string HashPassword(string password); 
    public bool  VerifyHashedPassword(string userPassword, string requestPassword);
}