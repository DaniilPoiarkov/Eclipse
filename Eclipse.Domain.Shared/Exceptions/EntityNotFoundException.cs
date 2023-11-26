namespace Eclipse.Domain.Shared.Exceptions;

public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(Type type) : base("Entity:NotFound", type.Name)
    {
    }
}
