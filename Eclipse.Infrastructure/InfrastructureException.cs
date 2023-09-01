namespace Eclipse.Infrastructure;

internal class InfrastructureException : Exception
{
    public InfrastructureException(string? message = null, Exception? inner = null)
        : base(message, inner) { }
}
