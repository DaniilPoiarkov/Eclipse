using Eclipse.Domain.Exceptions;

namespace Eclipse.Domain.IdentityUsers;

[Serializable]
internal sealed class DuplicateDataException : DomainException
{
    internal DuplicateDataException(string property, object value)
        : base("IdentityUser:DuplicateData", property, value.ToString() ?? string.Empty) { }
}
