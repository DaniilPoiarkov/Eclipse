using Eclipse.Application.Contracts.InboxMessages;
using Eclipse.Application.InboxMessages;

using NSubstitute;

using Quartz;

using Xunit;

namespace Eclipse.Application.Tests.InboxMessages;

public sealed class DeleteProcessedInboxMessagesJobTests
{
    private readonly IInboxMessageService _inboxMessageService;

    private readonly DeleteProcessedInboxMessagesJob _sut;

    public DeleteProcessedInboxMessagesJobTests()
    {
        _inboxMessageService = Substitute.For<IInboxMessageService>();
        _sut = new DeleteProcessedInboxMessagesJob(_inboxMessageService);
    }

    [Fact]
    public async Task Execute_WhenCalled_ThenDelegatesCallToService()
    {
        var context = Substitute.For<IJobExecutionContext>();

        await _sut.Execute(context);

        await _inboxMessageService.Received().DeleteProcessedAsync(context.CancellationToken);
    }
}
