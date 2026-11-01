using Eclipse.Application.Contracts.InboxMessages;

using Microsoft.Extensions.Configuration;

using Quartz;

namespace Eclipse.Application.InboxMessages;

internal sealed class ConvertOutboxToInboxJob : IJob
{
    private readonly IInboxMessageConvertor _inboxMessageConvertor;

    private readonly IConfiguration _configuration;

    public ConvertOutboxToInboxJob(IInboxMessageConvertor inboxMessageConvertor, IConfiguration configuration)
    {
        _inboxMessageConvertor = inboxMessageConvertor;
        _configuration = configuration;
    }

    public async ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        var count = _configuration.GetSection("InboxMessages")
            .GetValue<int>("ConvertCount");

        await _inboxMessageConvertor.ConvertAsync(count, cancellationToken);
    }
}
