namespace Domain.Exceptions;

public sealed class NotFoundException() : Exception("Entity not found")
{
}