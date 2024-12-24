using Eclipse.Application.Contracts.InboxMessages;

using Microsoft.Extensions.Configuration;

using Quartz;

namespace Eclipse.Application.InboxMessages;

// TODO: Add configuration
internal sealed class ConvertOutboxToInboxJob : IJob
{
    private readonly IInboxMessageConvertor _inboxMessageConvertor;

    private readonly IConfiguration _configuration;

    public ConvertOutboxToInboxJob(IInboxMessageConvertor inboxMessageConvertor, IConfiguration configuration)
    {
        _inboxMessageConvertor = inboxMessageConvertor;
        _configuration = configuration;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var count = _configuration.GetSection("OutboxMessages")
            .GetValue<int>("ProcessCount");

        await _inboxMessageConvertor.ConvertAsync(count, context.CancellationToken);
    }
}
