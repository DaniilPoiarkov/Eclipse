using Microsoft.Extensions.Diagnostics.HealthChecks;

using Newtonsoft.Json;

using System.Text;

namespace Eclipse.Infrastructure.Jobs.HealthCheck;

[Serializable]
internal sealed class HealthCheckEntry
{
    public Dictionary<string, object> Data { get; set; } = [];

    public TimeSpan Duration { get; set; }

    public HealthStatus Status { get; set; }

    public string[] Tags { get; set; } = [];

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
