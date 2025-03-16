namespace Eclipse.DataAccess.Migrations;

[AttributeUsage(AttributeTargets.Class)]
internal sealed class MigrationAttribute : Attribute
{
    public long Version { get; }

    public string? Description { get; }

    public MigrationAttribute(long version, string? description = null)
    {
        Version = version;
        Description = description;
    }
}
