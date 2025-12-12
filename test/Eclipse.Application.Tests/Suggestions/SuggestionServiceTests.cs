using Eclipse.Application.Contracts.Suggestions;
using Eclipse.Application.Google.Sheets;
using Eclipse.Application.Suggestions;
using Eclipse.Common.Clock;
using Eclipse.Domain.Suggestions;
using Eclipse.Domain.Users;
using Eclipse.Tests.Generators;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Suggestions;

public class SuggestionServiceTests
{
    private readonly IEclipseSheetsService<Suggestion> _sheetsService;

    private readonly IUserRepository _userRepository;

    private readonly ITimeProvider _timeProvider;

    private readonly SuggestionsService _sut;

    public SuggestionServiceTests()
    {
        _sheetsService = Substitute.For<IEclipseSheetsService<Suggestion>>();
        _userRepository = Substitute.For<IUserRepository>();
        _timeProvider = Substitute.For<ITimeProvider>();

        _sut = new SuggestionsService(_sheetsService, _userRepository, _timeProvider);
    }

    [Fact]
    public async Task GetWithUserInfo_WhenRequested_ThenSuggestionsWithUsersReturned()
    {
        var users = UserGenerator.Generate(5);
        _userRepository.GetAllAsync().Returns(users);

        var suggestions = SuggestionsGenerator.Generate(5, 1);
        _sheetsService.GetAllAsync().Returns(suggestions);

        var result = await _sut.GetWithUserInfo();
        result.All(r => r.User is not null && !r.Text.Contains(' ')).Should().BeTrue();
    }

    [Fact]
    public async Task CreateAsync_WhenCreated_ThenAddsSuggestion()
    {
        var utcNow = DateTime.UtcNow;
        _timeProvider.Now.Returns(utcNow);

        var suggestion = new CreateSuggestionRequest
        {
            TelegramUserId = 1,
            Text = "text"
        };

        var result = await _sut.CreateAsync(suggestion);

        result.IsSuccess.Should().BeTrue();

        await _sheetsService.Received().AddAsync(
            Arg.Is<Suggestion>(s => s.Text == suggestion.Text
                && s.TelegramUserId == suggestion.TelegramUserId
                && s.CreatedAt == utcNow)
        );
    }
}
