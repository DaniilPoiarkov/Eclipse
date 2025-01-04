using Eclipse.Application.OutboxMessages;
using Eclipse.Domain.OutboxMessages;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.OutboxMessages;

public sealed class OutboxMessagesServiceTests
{
    private readonly IOutboxMessageRepository _repository;

    private readonly OutboxMessagesService _sut;

    public OutboxMessagesServiceTests()
    {
        _repository = Substitute.For<IOutboxMessageRepository>();

        _sut = new(_repository);
    }

    [Fact]
    public async Task DeleteSuccessfullyProcessedAsync_WhenCalled_ThenDelegatesExecutionToRepository()
    {
        await _sut.DeleteSuccessfullyProcessedAsync();
        await _repository.Received().DeleteSuccessfullyProcessedAsync();
    }
}
