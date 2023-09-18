namespace Eclipse.Domain.Base;

public abstract class Entity
{
    public Guid Id { get; private set; }

    public Entity(Guid id)
    {
        Id = id;
    }

    protected Entity() { }
}
