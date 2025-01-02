using Eclipse.Application.Contracts.InboxMessages;
using Eclipse.Application.InboxMessages;
using Eclipse.Application.Tests.InboxMessages.Utils;
using Eclipse.Common.Clock;
using Eclipse.Common.Events;
using Eclipse.Domain.InboxMessages;
using Eclipse.Domain.Shared.InboxMessages;

using FluentAssertions;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using Xunit;

namespace Eclipse.Application.Tests.InboxMessages;

public sealed class TypedInboxMessageProcessorTests
{
    private readonly IInboxMessageRepository _repository;

    private readonly IEventHandler<TestEvent> _eventHandler;

    private readonly ITimeProvider _timeProvider;

    private readonly ILogger<TypedInboxMessageProcessor<TestEvent, IEventHandler<TestEvent>>> _logger;

    private readonly TypedInboxMessageProcessor<TestEvent, IEventHandler<TestEvent>> _sut;

    public TypedInboxMessageProcessorTests()
    {
        _repository = Substitute.For<IInboxMessageRepository>();
        _eventHandler = Substitute.For<IEventHandler<TestEvent>>();
        _timeProvider = Substitute.For<ITimeProvider>();
        _logger = Substitute.For<ILogger<TypedInboxMessageProcessor<TestEvent, IEventHandler<TestEvent>>>>();

        _sut = new TypedInboxMessageProcessor<TestEvent, IEventHandler<TestEvent>>(_repository, _eventHandler, _timeProvider, _logger);
    }

    [Theory]
    [InlineData(10)]
    public async Task ProcessAsync_WhenNoMessages_ThenReturnsEmptyResult(int count)
    {
        _repository.GetPendingAsync(count, _eventHandler.GetType().FullName).Returns([]);

        var result = await _sut.ProcessAsync(count);

        result.Should().Be(ProcessInboxMessagesResult.Empty);
    }

    [Theory]
    [InlineData(10)]
    public async Task ProcessAsync_WhenProcessedSuccessfully_ThenSetsProcessedStatus(int count)
    {
        var utcNow = DateTime.UtcNow;
        _timeProvider.Now.Returns(utcNow);

        var message = InboxMessage.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            typeof(TestEventHandler).FullName!,
            JsonConvert.SerializeObject(new TestEvent(Guid.NewGuid()), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
            }),
            typeof(TestEvent).AssemblyQualifiedName!,
            utcNow
        );

        var messages = new List<InboxMessage>
        {
            message
        };

        _repository.GetPendingAsync(count, _eventHandler.GetType().FullName).Returns(messages);

        var expected = new ProcessInboxMessagesResult(1, 1, 0, []);

        var result = await _sut.ProcessAsync(count);

        result.Should().BeEquivalentTo(expected);

        await _repository.Received(2).UpdateRangeAsync(messages);
        await _eventHandler.Received().Handle(Arg.Any<TestEvent>());

        message.Status.Should().Be(InboxMessageStatus.Processed);
        message.ProcessedAt.Should().Be(utcNow);
    }

    [Theory]
    [InlineData(10)]
    public async Task ProcessAsync_WhenExceptionThrown_ThenSetsFailedStatus(int count)
    {
        var utcNow = DateTime.UtcNow;
        _timeProvider.Now.Returns(utcNow);

        var message = InboxMessage.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            typeof(TestEventHandler).FullName!,
            JsonConvert.SerializeObject(new TestEvent(Guid.NewGuid()), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
            }),
            typeof(TestEvent).AssemblyQualifiedName!,
            utcNow
        );

        var messages = new List<InboxMessage>
        {
            message
        };

        _repository.GetPendingAsync(count, _eventHandler.GetType().FullName).Returns(messages);

        var exception = new Exception("Test exception");

        _eventHandler.Handle(Arg.Any<TestEvent>()).Throws(exception);

        await _sut.ProcessAsync(count);

        await _repository.Received(2).UpdateRangeAsync(messages);
        await _eventHandler.Received().Handle(Arg.Any<TestEvent>());

        message.Status.Should().Be(InboxMessageStatus.Failed);
        message.ProcessedAt.Should().Be(utcNow);
        message.Error.Should().NotBeNullOrEmpty();
    }
}
