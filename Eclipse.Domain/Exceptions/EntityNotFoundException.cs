namespace Eclipse.Domain.Exceptions;

internal class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(Type type) : base("Entity {0} not found", type.Name)
    {
    }
}
