using Newtonsoft.Json;

namespace Eclipse.Domain.Entities;

/// <summary>
/// Base entity class
/// </summary>
public abstract class Entity
{
    [JsonProperty(PropertyName = "id")]
    public Guid Id { get; private set; }

    public Entity(Guid id)
    {
        Id = id;
    }

    protected Entity() { }
}
