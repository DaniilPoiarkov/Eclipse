using HealthChecks.UI.Core;

using Newtonsoft.Json;

using System.Text;

namespace Eclipse.Infrastructure.Internals.Quartz.Models;

internal class HealthCheckEntry
{
    public Dictionary<string, object> Data { get; set; } = new();

    public TimeSpan Duration { get; set; }

    public UIHealthStatus Status { get; set; }

    public string[] Tags { get; set; } = Array.Empty<string>();

    public override string ToString()
    {
        var sb = new StringBuilder()
            .AppendLine($"Status: {Status}");

        foreach (var kv in Data)
        {
            sb.AppendLine($"{kv.Key}: {JsonConvert.SerializeObject(kv.Value)}");
        }

        sb.AppendLine($"Tags: {string.Join(", ", Tags)}");

        return sb.ToString();
    }
}
