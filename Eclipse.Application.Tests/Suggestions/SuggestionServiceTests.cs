using Eclipse.Application.Contracts.Google.Sheets.Suggestions;
using Eclipse.Application.Contracts.Suggestions;
using Eclipse.Application.Contracts.Telegram.TelegramUsers;
using Eclipse.Application.Suggestions;
using Eclipse.Tests.Builders;

namespace Eclipse.Application.Tests.Suggestions;

public class SuggestionServiceTests
{
    private readonly ISuggestionsService _sut;

    public SuggestionServiceTests()
    {
        var suggestionsSheetsService = Substitute.For<ISuggestionsSheetsService>();
        var suggestions = SuggestionsBuilder.Generate(5, 1);

        suggestionsSheetsService.GetAll().Returns(suggestions);

        var userRepository = Substitute.For<ITelegramUserRepository>();
        var users = TelegramUserBuilder.GenerateUsers(1, 5);

        userRepository.GetAll().Returns(users);

        _sut = new SuggestionsService(suggestionsSheetsService, userRepository);
    }

    [Fact]
    public void GetWithUserInfo_WhenRequested_ThenSuggestionsWithUsersReturned()
    {
        var result = _sut.GetWithUserInfo();
        result.All(r => r.User is not null && !r.Text.Contains(' ')).Should().BeTrue();
    }
}
