namespace Eclipse.Domain.Shared.Entities;

/// <summary>
/// Base entity class
/// </summary>
public abstract class Entity
{
    public Guid Id { get; private set; }

    public Entity(Guid id)
    {
        Id = id;
    }

    protected Entity() { }

    public override string ToString()
    {
        return $"Id: {Id}";
    }
}
