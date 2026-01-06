namespace Domain.Exceptions;

public sealed class AlreadyExistException(string message)
    : Exception(message);