using Eclipse.Application.Contracts.OutboxMessages;
using Microsoft.Extensions.Configuration;

using Quartz;

namespace Eclipse.Application.OutboxMessages.ProcessMessages;

internal sealed class ProcessOutboxMessagesJob : IJob
{
    private readonly IOutboxMessagesService _outboxMessagesService;

    private readonly IConfiguration _configuration;

    public ProcessOutboxMessagesJob(IOutboxMessagesService outboxMessagesService, IConfiguration configuration)
    {
        _outboxMessagesService = outboxMessagesService;
        _configuration = configuration;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var count = _configuration.GetSection("OutboxMessages")
            .GetValue<int>("ProcessCount");

        var res = await _outboxMessagesService.ProcessAsync(count, context.CancellationToken);
    }
}
