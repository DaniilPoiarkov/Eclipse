using Eclipse.Application.Google.Sheets;
using Eclipse.Common.Clock;
using Eclipse.Common.Sheets;
using Eclipse.Domain.OutboxMessages;
using Eclipse.Domain.Suggestions;

using FluentAssertions;

using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Google.Sheets;

public sealed class SuggestionsSheetsServiceTests
{
    private readonly ISheetsService _sheetsService;

    private readonly IConfiguration _configuration;

    private readonly IOutboxMessageRepository _outboxMessageRepository;

    private readonly ITimeProvider _timeProvider;

    private readonly SuggestionsSheetsService _sut;

    public SuggestionsSheetsServiceTests()
    {
        _sheetsService = Substitute.For<ISheetsService>();
        _configuration = Substitute.For<IConfiguration>();
        _outboxMessageRepository = Substitute.For<IOutboxMessageRepository>();
        _timeProvider = Substitute.For<ITimeProvider>();

        _sut = new SuggestionsSheetsService(_sheetsService, new SuggestionParser(), _configuration, _outboxMessageRepository, _timeProvider);
    }

    [Theory]
    [InlineData("Text", 1)]
    public async Task GetAllAsync_WhenCalled_ThenReturnsSuggestions(string text, long chatid)
    {
        var suggestion = Suggestion.Create(Guid.NewGuid(), text, chatid, DateTime.UtcNow);

        _sheetsService.GetAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<IObjectParser<Suggestion>>()
        ).Returns([suggestion]);

        var suggestions = await _sut.GetAllAsync();

        suggestions.Should().BeEquivalentTo([suggestion]);
    }

    [Theory]
    [InlineData("text", 1)]
    public async Task AddAsync_WhenItemAdded_ThenEventsTriggered(string text, long chatId)
    {
        var utcNow = DateTime.UtcNow;
        _timeProvider.Now.Returns(utcNow);

        var suggestion = Suggestion.Create(Guid.NewGuid(), text, chatId, utcNow);

        var expectedOutboxMessages = suggestion.GetEvents()
            .Select(@event => new OutboxMessage(
                Guid.CreateVersion7(),
                @event.GetType().AssemblyQualifiedName!,
                JsonConvert.SerializeObject(@event, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                }),
                utcNow
            ))
            .ToList();

        await _sut.AddAsync(suggestion);

        await _outboxMessageRepository.Received().CreateRangeAsync(
            Arg.Is<IEnumerable<OutboxMessage>>(messages =>
                messages.All(message =>
                    expectedOutboxMessages.Exists(expected => message.Type == expected.Type
                        && message.JsonContent == expected.JsonContent
                        && message.OccuredAt == expected.OccuredAt
                    )
                )
            )
        );

        await _sheetsService.Received().AppendAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            suggestion,
            Arg.Any<IObjectParser<Suggestion>>()
        );
    }
}
