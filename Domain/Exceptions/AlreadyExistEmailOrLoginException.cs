namespace Domain.Exceptions;

public sealed class AlreadyExistEmailOrLoginException(string emailOrLogin)
    : Exception($"User with email or login '{emailOrLogin}' already exists.");