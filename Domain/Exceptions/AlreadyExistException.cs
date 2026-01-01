namespace Domain.Exceptions;

public sealed class AlreadyExistException(string emailOrLogin)
    : Exception($"User with email or login '{emailOrLogin}' already exists.");