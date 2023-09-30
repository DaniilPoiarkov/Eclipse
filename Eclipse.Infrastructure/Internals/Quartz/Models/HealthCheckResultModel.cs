using HealthChecks.UI.Core;

namespace Eclipse.Infrastructure.Internals.Quartz.Models;

internal class HealthCheckResultModel
{
    public UIHealthStatus Status { get; set; }

    public TimeSpan TotalDuration { get; set; }

    public Dictionary<string, HealthCheckEntry> Entries { get; set; } = new();
}
