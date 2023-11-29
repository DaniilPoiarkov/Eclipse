using HealthChecks.UI.Core;

namespace Eclipse.Infrastructure.Jobs.HealthCheck;

[Serializable]
internal sealed class HealthCheckResultModel
{
    public UIHealthStatus Status { get; set; }

    public TimeSpan TotalDuration { get; set; }

    public Dictionary<string, HealthCheckEntry> Entries { get; set; } = [];
}
