namespace Domain.Exceptions;

public sealed class InvalidCredentialsException(string? message) : Exception($"Invalid credentials. {message}");