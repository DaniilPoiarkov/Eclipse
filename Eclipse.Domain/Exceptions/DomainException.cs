using Eclipse.Infrastructure.Exceptions;

namespace Eclipse.Domain.Exceptions;

public class DomainException : EclipseException
{
    public DomainException(string message, params string[] args) : base(message)
    {
        for (int i = 0; i < args.Length; i++)
        {
            WithData($"{i}", args[i]);
        }
    }
}
