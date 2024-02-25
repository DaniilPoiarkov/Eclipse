using Eclipse.Common.Exceptions;

namespace Eclipse.Domain.Exceptions;

[Serializable]
public sealed class EntityNotFoundException : EclipseException
{
    public EntityNotFoundException(Type type) : base("Entity:NotFound")
    {
        WithData("{0}", type.Name);
    }
}
