using Eclipse.Localization.Exceptions;

namespace Eclipse.Domain.Exceptions;

public class DomainException : LocalizedException
{
    public DomainException(string message, params string[] args) : base(message, args)
    {
    }
}
