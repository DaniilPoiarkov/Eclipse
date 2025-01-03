using Eclipse.Application.Contracts.InboxMessages;
using Eclipse.Application.InboxMessages;
using Eclipse.Application.Tests.InboxMessages.Utils;

using Microsoft.Extensions.Configuration;

using NSubstitute;

using Quartz;

using Xunit;

namespace Eclipse.Application.Tests.InboxMessages;

public sealed class ProcessTypedInboxMessagesJobTests
{
    private readonly IInboxMessageProcessor<TestEvent, TestEventHandler> _processor;

    private readonly IConfiguration _configuration;

    private readonly ProcessTypedInboxMessagesJob<TestEvent, TestEventHandler> _sut;

    public ProcessTypedInboxMessagesJobTests()
    {
        _processor = Substitute.For<IInboxMessageProcessor<TestEvent, TestEventHandler>>();
        _configuration = Substitute.For<IConfiguration>();

        _sut = new ProcessTypedInboxMessagesJob<TestEvent, TestEventHandler>(_processor, _configuration);
    }

    [Theory]
    [InlineData(1)]
    public async Task Execute_WhenTriggered_ThenDelegatesCallToProcessor(int count)
    {
        var section = Substitute.For<IConfigurationSection>();
        section.GetSection("ProcessCount").Value.Returns($"{count}");

        _configuration.GetSection("InboxMessages").Returns(section);

        var context = Substitute.For<IJobExecutionContext>();

        await _sut.Execute(context);

        _configuration.Received().GetSection("InboxMessages");
        section.Received().GetValue<int>("ProcessCount");

        await _processor.Received().ProcessAsync(count, context.CancellationToken);
    }
}
