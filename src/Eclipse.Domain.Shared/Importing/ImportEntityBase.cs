namespace Eclipse.Domain.Shared.Importing;

public abstract class ImportEntityBase
{
    public Guid Id { get; set; }

    public string? Exception { get; set; }

    public bool CanBeImported()
    {
        return Exception.IsNullOrEmpty();
    }
}
