using Eclipse.Application.Contracts.OutboxMessages;
using Eclipse.Application.OutboxMessages.Jobs;

using NSubstitute;

using Quartz;

using Xunit;

namespace Eclipse.Application.Tests.OutboxMessages;

public sealed class DeleteSuccessfullyProcessedOutboxMessagesJobTests
{
    private readonly IOutboxMessagesService _outboxMessagesService;

    private readonly DeleteSuccessfullyProcessedOutboxMessagesJob _sut;

    public DeleteSuccessfullyProcessedOutboxMessagesJobTests()
    {
        _outboxMessagesService = Substitute.For<IOutboxMessagesService>();
        _sut = new DeleteSuccessfullyProcessedOutboxMessagesJob(_outboxMessagesService);
    }

    [Fact]
    public async Task Execute_WhenTriggered_ThenSimplyDelegateExecutionToService()
    {
        var context = Substitute.For<IJobExecutionContext>();
        await _sut.Execute(context);

        await _outboxMessagesService.Received().DeleteSuccessfullyProcessedAsync();
    }
}
