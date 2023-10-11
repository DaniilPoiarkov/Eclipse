using Eclipse.Domain.Shared.Exceptions;

namespace Eclipse.Domain.Shared.IdentityUsers;

public class DuplicateDataException : DomainException
{
    public DuplicateDataException(string property, object value)
        : base("$User with {0} {1} already exists", property, value.ToString() ?? string.Empty) { }
}
