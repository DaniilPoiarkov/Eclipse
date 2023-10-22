namespace Eclipse.Domain.Shared.Entities;

public abstract class AggregateRoot : Entity
{
    public AggregateRoot(Guid id)
        : base(id) { }

    public AggregateRoot() { }
}
