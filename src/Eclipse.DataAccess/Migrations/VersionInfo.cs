namespace Eclipse.DataAccess.Migrations;

internal sealed class VersionInfo
{
    public Guid Id { get; init; }

    public long Version { get; init; }

    public string? Description { get; init; }

    public DateTime AppliedAt { get; init; }
}
