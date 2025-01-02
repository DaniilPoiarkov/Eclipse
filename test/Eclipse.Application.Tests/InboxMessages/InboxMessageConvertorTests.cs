using Eclipse.Application.Contracts.InboxMessages;
using Eclipse.Application.InboxMessages;
using Eclipse.Application.Tests.InboxMessages.Utils;
using Eclipse.Common.Clock;
using Eclipse.Common.Events;
using Eclipse.Domain.InboxMessages;
using Eclipse.Domain.OutboxMessages;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.InboxMessages;

public sealed class InboxMessageConvertorTests
{
    private readonly IOutboxMessageRepository _outboxMessageRepository;

    private readonly IInboxMessageRepository _inboxMessageRepository;

    private readonly IServiceProvider _serviceProvider;

    private readonly ITimeProvider _timeProvider;

    private readonly ILogger<InboxMessageConvertor> _logger;

    private readonly InboxMessageConvertor _sut;

    public InboxMessageConvertorTests()
    {
        _outboxMessageRepository = Substitute.For<IOutboxMessageRepository>();
        _inboxMessageRepository = Substitute.For<IInboxMessageRepository>();
        _serviceProvider = Substitute.For<IServiceProvider>();
        _timeProvider = Substitute.For<ITimeProvider>();
        _logger = Substitute.For<ILogger<InboxMessageConvertor>>();

        _sut = new InboxMessageConvertor(_outboxMessageRepository, _inboxMessageRepository, _serviceProvider, _timeProvider, _logger);
    }

    [Theory]
    [InlineData(1)]
    public async Task ConvertAsync_WhenNoOutboxMessages_ThenReturnsEmptyResult(int count)
    {
        _outboxMessageRepository.GetNotProcessedAsync(count, Arg.Any<CancellationToken>()).Returns([]);

        var result = await _sut.ConvertAsync(count);

        result.Should().Be(OutboxToInboxConversionResult.Empty);
    }

    [Theory]
    [InlineData(1)]
    public async Task ConvertAsync_WhenHasOutboxMessages_ThenConvertsToInboxForEachHandler(int count)
    {
        var utcNow = DateTime.UtcNow;
        _timeProvider.Now.Returns(utcNow);

        var outboxMessages = new[]
        {
            new TestEvent(Guid.NewGuid()),
            new TestEvent(Guid.NewGuid()),
        }.Select(e =>
            new OutboxMessage(
                Guid.NewGuid(),
                e.GetType().AssemblyQualifiedName!,
                JsonConvert.SerializeObject(e, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                }),
                utcNow
            )
        ).ToList();

        var handlers = new IEventHandler<TestEvent>[]
        {
            new TestEventHandler(),
            new AnotherTestEventHandler()
        };

        var factory = Substitute.For<IServiceScopeFactory>();
        _serviceProvider.GetService(typeof(IServiceScopeFactory)).Returns(factory);

        var scope = Substitute.For<IServiceScope>();
        factory.CreateScope().Returns(scope);

        var serviceProvider = Substitute.For<IServiceProvider>();
        scope.ServiceProvider.Returns(serviceProvider);

        serviceProvider
            .GetService(typeof(IEnumerable<IEventHandler<TestEvent>>))
            .Returns(handlers);

        var expectedInboxMessages = outboxMessages
            .GroupJoin(handlers,
                outboxMessage => outboxMessage.Type,
                _ => typeof(TestEvent).AssemblyQualifiedName,
                (outboxMessage, handlers) => handlers.Select(handler =>
                    InboxMessage.Create(
                        Guid.CreateVersion7(),
                        outboxMessage.Id,
                        handler.GetType().FullName!,
                        outboxMessage.JsonContent,
                        outboxMessage.Type,
                        outboxMessage.OccuredAt
                    )
                )
            )
            .SelectMany(i => i)
            .ToList();

        _outboxMessageRepository.GetNotProcessedAsync(count).Returns(outboxMessages);

        var expected = new OutboxToInboxConversionResult(outboxMessages.Count, outboxMessages.Count, 0, []);

        var result = await _sut.ConvertAsync(count);

        result.Should().BeEquivalentTo(expected);

        outboxMessages.Should().AllSatisfy(outboxMessage =>
        {
            outboxMessage.ProcessedAt.Should().Be(utcNow);
            outboxMessage.Error.Should().BeNullOrEmpty();
        });

        await _inboxMessageRepository.Received()
            .CreateRangeAsync(
                Arg.Is<List<InboxMessage>>(messages =>
                    messages.All(m =>
                        expectedInboxMessages.Exists(e => e.OutboxMessageId == m.OutboxMessageId
                            && e.HandlerName == m.HandlerName)
                    )
                ),
                Arg.Any<CancellationToken>()
            );

        await _outboxMessageRepository.Received().UpdateRangeAsync(outboxMessages);
    }

    [Theory]
    [InlineData(1)]
    public async Task ConvertAsync_WhenPayloadTypeCannotBeResolved_ThenErrorSet(int count)
    {
        var utcNow = DateTime.UtcNow;
        _timeProvider.Now.Returns(utcNow);

        var outboxMessage = new OutboxMessage(
            Guid.NewGuid(),
            typeof(TestEvent).Name,
            JsonConvert.SerializeObject(new TestEvent(Guid.NewGuid()), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
            }),
            utcNow
        );

        var factory = Substitute.For<IServiceScopeFactory>();
        _serviceProvider.GetService(typeof(IServiceScopeFactory)).Returns(factory);

        var scope = Substitute.For<IServiceScope>();
        factory.CreateScope().Returns(scope);

        List<OutboxMessage> outboxMessages = [ outboxMessage ];

        _outboxMessageRepository.GetNotProcessedAsync(count).Returns(outboxMessages);

        var expected = new OutboxToInboxConversionResult(1, 0, 1, [ "Cannot resolve payload type during converting to inbox message." ]);

        var result = await _sut.ConvertAsync(count);

        result.Should().BeEquivalentTo(expected);

        outboxMessage.ProcessedAt.Should().Be(utcNow);
        outboxMessage.Error.Should().Be("Cannot resolve payload type during converting to inbox message.");

        await _inboxMessageRepository.Received().CreateRangeAsync(
            Arg.Is<List<InboxMessage>>(messages => messages.IsNullOrEmpty()),
            Arg.Any<CancellationToken>()
        );

        await _outboxMessageRepository.Received().UpdateRangeAsync(outboxMessages);

        _logger.Received().Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>()
        );
    }
}
