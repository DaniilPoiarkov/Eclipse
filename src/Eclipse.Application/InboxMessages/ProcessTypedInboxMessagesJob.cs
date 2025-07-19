using Eclipse.Application.Contracts.InboxMessages;
using Eclipse.Common.Events;

using Microsoft.Extensions.Configuration;

using Quartz;

namespace Eclipse.Application.InboxMessages;

internal sealed class ProcessTypedInboxMessagesJob<TEvent, TEventHandler> : IJob
    where TEvent : IDomainEvent
    where TEventHandler : IEventHandler<TEvent>
{
    private readonly IInboxMessageProcessor<TEvent, TEventHandler> _processor;

    private readonly IConfiguration _configuration;

    public ProcessTypedInboxMessagesJob(IInboxMessageProcessor<TEvent, TEventHandler> processor, IConfiguration configuration)
    {
        _processor = processor;
        _configuration = configuration;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var count = _configuration.GetSection("InboxMessages")
            .GetValue<int>("ProcessCount");

        await _processor.ProcessAsync(count, context.CancellationToken);
    }
}
