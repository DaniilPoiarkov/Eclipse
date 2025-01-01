using Eclipse.Application.Contracts.InboxMessages;

using Microsoft.Extensions.Configuration;

using Quartz;

namespace Eclipse.Application.InboxMessages;

// TODO: Remove
//internal sealed class ProcessInboxMessagesJob : IJob
//{
//    private readonly IInboxMessageProcessor _processor;

//    private readonly IConfiguration _configuration;

//    public ProcessInboxMessagesJob(IInboxMessageProcessor processor, IConfiguration configuration)
//    {
//        _processor = processor;
//        _configuration = configuration;
//    }

//    public async Task Execute(IJobExecutionContext context)
//    {
//        var count = _configuration.GetSection("InboxMessages")
//            .GetValue<int>("ProcessCount");

//        await _processor.ProcessAsync(count, context.CancellationToken);
//    }
//}
