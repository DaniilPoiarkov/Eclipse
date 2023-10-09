namespace Eclipse.Domain.Shared.Entities;

public class AggregateRoot : Entity
{
    public AggregateRoot(Guid id)
        : base(id) { }

    public AggregateRoot() { }
}
