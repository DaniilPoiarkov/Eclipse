using Newtonsoft.Json;

namespace Eclipse.DataAccess.Migrations;

internal sealed class VersionInfo
{
    public long Version { get; init; }

    public string? Description { get; init; }

    public DateTime AppliedAt { get; init; }
}
