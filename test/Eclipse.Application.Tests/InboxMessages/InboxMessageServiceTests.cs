using Eclipse.Application.InboxMessages;
using Eclipse.Domain.InboxMessages;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.InboxMessages;

public sealed class InboxMessageServiceTests
{
    private readonly IInboxMessageRepository _repository;

    private readonly InboxMessageService _sut;

    public InboxMessageServiceTests()
    {
        _repository = Substitute.For<IInboxMessageRepository>();
        _sut = new InboxMessageService(_repository);
    }

    [Fact]
    public async Task DeleteProcessedAsync_WhenCalled_ThenDelegatesCallToRepository()
    {
        var cancellationToken = new CancellationToken();
        await _sut.DeleteProcessedAsync(cancellationToken);
        await _repository.Received().DeleteSuccessfullyProcessedAsync(cancellationToken);
    }
}
