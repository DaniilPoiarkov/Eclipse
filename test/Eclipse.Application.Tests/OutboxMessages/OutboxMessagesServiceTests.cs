using Eclipse.Application.Contracts.OutboxMessages;
using Eclipse.Application.OutboxMessages;
using Eclipse.Common.Clock;
using Eclipse.Common.EventBus;
using Eclipse.Common.Events;
using Eclipse.Domain.OutboxMessages;
using Eclipse.Tests.OutboxMessages;

using FluentAssertions;

using Microsoft.Extensions.Logging;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
    public async Task ProcessAsync_WhenNoMessagesFound_ThenEmptyResultReturned()
    {
        _repository.GetNotProcessedAsync(Arg.Any<int>()).Returns([]);

        var result = await _sut.ProcessAsync(Arg.Any<int>());

        result.Should().Be(ProcessOutboxMessagesResult.Empty);
    }

    [Fact]
    public async Task DeleteSuccessfullyProcessedAsync_WhenCalled_ThenDelegatesExecutionToRepository()
    {
        await _sut.DeleteSuccessfullyProcessedAsync();
        await _repository.Received().DeleteSuccessfullyProcessedAsync();
    }

    [Fact]
    public async Task ProcessAsync_WhenMessagesValid_ThenProcessedAtSet()
    {
        var count = 10;
        var now = DateTime.UtcNow;
        var messages = OutboxMessageGenerator.Generate(count).ToList();

        _repository.GetNotProcessedAsync(count).Returns(messages);
        _timeProvider.Now.Returns(now);

        var result = await _sut.ProcessAsync(count);

        result.TotalCount.Should().Be(count);
        result.ProcessedCount.Should().Be(count);
        result.FailedCount.Should().Be(0);
        result.OcurredErrors.Should().BeEmpty();

        messages.All(m => m.ProcessedAt == now && m.Error is null).Should().BeTrue();
        await _repository.Received().UpdateRangeAsync(messages);
    }

    [Fact]
    public async Task ProcessAsync_WhenExceptionOccures_ThenErrorsAreSetToMessage()
    {
        var count = 10;
        var now = DateTime.UtcNow;
        var error = "some error";

        var messages = OutboxMessageGenerator.Generate(count).ToList();

        _repository.GetNotProcessedAsync(count).Returns(messages);
        _timeProvider.Now.Returns(now);
        _eventBus.Publish(Arg.Any<IDomainEvent>()).Throws(new Exception(error));

        var result = await _sut.ProcessAsync(count);

        result.TotalCount.Should().Be(count);
        result.FailedCount.Should().Be(count);
        result.ProcessedCount.Should().Be(0);
        result.OcurredErrors.Should().BeEquivalentTo(messages.Select(m => m.Error));

        messages.All(m => m.ProcessedAt == now && m.Error == error).Should().BeTrue();

        await _repository.Received().UpdateRangeAsync(messages);
    }

    [Fact]
    public async Task ProcessAsync_WhenMessageHasInvalidJsonContent_ThenErrorSet()
    {
        var count = 1;
        var now = DateTime.UtcNow;

        var message = new OutboxMessage(Guid.NewGuid(), "invalid_type", "invalid_content", now);
        List<OutboxMessage> messages = [message];

        _repository.GetNotProcessedAsync(count).Returns(messages);
        _timeProvider.Now.Returns(now);

        var result = await _sut.ProcessAsync(count);

        result.TotalCount.Should().Be(count);
        result.FailedCount.Should().Be(count);
        result.ProcessedCount.Should().Be(0);
        result.OcurredErrors.Should().BeEquivalentTo([message.Error]);

        message.Error.Should().NotBeNullOrEmpty();
        message.ProcessedAt.Should().Be(now);

        await _repository.Received().UpdateRangeAsync(messages);
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("[]")]
    [InlineData("""{ "name": "name" }""")]
    [InlineData("""{ "age": 15 }""")]
    [InlineData("""{ "inner": { "forecast": "$5" } }""")]
    public async Task ProcessAsync_WhenJsonContentCannotBeDeserialized_ThenErrorSet(string json)
    {
        var count = 1;
        var now = DateTime.UtcNow;

        var message = new OutboxMessage(Guid.NewGuid(), "invalid_type", json, now);
        List<OutboxMessage> messages = [message];

        _repository.GetNotProcessedAsync(count).Returns(messages);
        _timeProvider.Now.Returns(now);

        var result = await _sut.ProcessAsync(count);

        result.TotalCount.Should().Be(count);
        result.FailedCount.Should().Be(count);
        result.ProcessedCount.Should().Be(0);
        result.OcurredErrors.Should().BeEquivalentTo([message.Error]);

        message.Error.Should().NotBeNullOrEmpty();
        message.ProcessedAt.Should().Be(now);

        await _repository.Received().UpdateRangeAsync(messages);
    }
}
