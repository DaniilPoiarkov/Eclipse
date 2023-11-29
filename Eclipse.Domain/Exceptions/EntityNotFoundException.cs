namespace Eclipse.Domain.Exceptions;

[Serializable]
public sealed class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(Type type) : base("Entity:NotFound", type.Name)
    {
    }
}
