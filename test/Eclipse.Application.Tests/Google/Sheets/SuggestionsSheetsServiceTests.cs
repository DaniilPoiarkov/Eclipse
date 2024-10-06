using Eclipse.Application.Google.Sheets;
using Eclipse.Common.EventBus;
using Eclipse.Common.Sheets;
using Eclipse.Domain.Suggestions;

using FluentAssertions;

using Microsoft.Extensions.Configuration;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Google.Sheets;

public sealed class SuggestionsSheetsServiceTests
{
    private readonly ISheetsService _sheetsService;

    private readonly IConfiguration _configuration;

    private readonly IEventBus _eventBus;

    private readonly SuggestionsSheetsService _sut;

    public SuggestionsSheetsServiceTests()
    {
        _sheetsService = Substitute.For<ISheetsService>();
        _configuration = Substitute.For<IConfiguration>();
        _eventBus = Substitute.For<IEventBus>();

        _sut = new SuggestionsSheetsService(_sheetsService, new SuggestionParser(), _configuration, _eventBus);
    }

    [Fact]
    public async Task GetAllAsync_WhenCalled_ThenReturnsSuggestions()
    {
        var suggestion = Suggestion.Create(Guid.NewGuid(), "suggestion", 1, DateTime.UtcNow);

        _sheetsService.GetAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<IObjectParser<Suggestion>>()
        ).Returns([suggestion]);

        var result = await _sut.GetAllAsync();

        result.Count.Should().Be(1);
        result[0].Should().Be(suggestion);
    }

    [Fact]
    public async Task AddAsync_WhenItemAdded_ThenEventsTriggered()
    {
        var suggestion = Suggestion.Create(Guid.NewGuid(), "suggestion", 1, DateTime.UtcNow);

        var @event = suggestion.GetEvents()[0];

        await _sut.AddAsync(suggestion);

        await _eventBus.Received(1).Publish(@event);

        await _sheetsService.Received(1).AppendAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            suggestion,
            Arg.Any<IObjectParser<Suggestion>>()
        );
    }
}
