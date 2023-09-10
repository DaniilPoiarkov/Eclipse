namespace Eclipse.Infrastructure.Exceptions;

internal class InfrastructureException : Exception
{
    public InfrastructureException(string? message = null, Exception? inner = null)
        : base(message, inner) { }
}
