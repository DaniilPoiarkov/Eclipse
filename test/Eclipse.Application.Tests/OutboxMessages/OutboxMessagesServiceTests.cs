using Eclipse.Application.OutboxMessages;
using Eclipse.Common.Clock;
using Eclipse.Common.EventBus;
using Eclipse.Domain.OutboxMessages;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.OutboxMessages;

public sealed class OutboxMessagesServiceTests
{
    private readonly IOutboxMessageRepository _repository;

    private readonly IEventBus _eventBus;

    private readonly ITimeProvider _timeProvider;

    private readonly OutboxMessagesService _sut;

    public OutboxMessagesServiceTests()
    {
        _repository = Substitute.For<IOutboxMessageRepository>();
        _eventBus = Substitute.For<IEventBus>();
        _timeProvider = Substitute.For<ITimeProvider>();
        var logger = Substitute.For<ILogger<OutboxMessagesService>>();

        _sut = new(_repository, _eventBus, _timeProvider, logger);
    }

    [Fact]
    public async Task DeleteSuccessfullyProcessedAsync_WhenCalled_ThenDelegatesExecutionToRepository()
    {
        await _sut.DeleteSuccessfullyProcessedAsync();
        await _repository.Received().DeleteSuccessfullyProcessedAsync();
    }
}
