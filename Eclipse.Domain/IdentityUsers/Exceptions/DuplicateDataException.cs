using Eclipse.Domain.Exceptions;

namespace Eclipse.Domain.IdentityUsers.Exceptions;

internal class DuplicateDataException : DomainException
{
    public DuplicateDataException(string property, object value)
        : base("$User with {0} {1} already exists", property, value.ToString() ?? string.Empty) { }
}
