﻿using HealthChecks.UI.Core;

using Microsoft.Extensions.Diagnostics.HealthChecks;

using Newtonsoft.Json;

using Quartz;

using Serilog;

namespace Eclipse.Infrastructure.Jobs.HealthCheck;

internal class HealthCheckJob : IJob
{
    private readonly ILogger _logger;

    private readonly HttpClient _client;

    public HealthCheckJob(ILogger logger, HttpClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var response = await _client.GetAsync("/_health-checks", context.CancellationToken);
        var json = await response.Content.ReadAsStringAsync(context.CancellationToken);

        var result = JsonConvert.DeserializeObject<HealthCheckResultModel>(json);

        if (result is null)
        {
            _logger.Error("Health check result is not available");
            return;
        }

        if (result.Status == UIHealthStatus.Healthy)
        {
            _logger.Information("Health status is {status}", result.Status);
            return;
        }

        var data = result.Entries
            .Where(e => e.Value.Status != HealthStatus.Healthy)
            .Select(e => $"{e.Key}: {e.Value}");

        _logger.Warning("Health status is {health}\n\r\n\rData: {data}", result.Status, string.Join(Environment.NewLine, data));
    }
}
