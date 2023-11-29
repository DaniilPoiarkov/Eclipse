using Eclipse.Domain.Exceptions;

namespace Eclipse.Domain.IdentityUsers;

[Serializable]
internal sealed class DuplicateDataException : DomainException
{
    internal DuplicateDataException(string property, object value)
        : base("$User with {0} {1} already exists", property, value.ToString() ?? string.Empty) { }
}
