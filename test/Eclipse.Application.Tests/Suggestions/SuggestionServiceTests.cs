using Eclipse.Application.Contracts.Google.Sheets.Suggestions;
using Eclipse.Application.Contracts.Suggestions;
using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Suggestions;

using FluentAssertions;

using NSubstitute;

using Xunit;
using Eclipse.Tests.Generators;
using Eclipse.Application.Contracts.Telegram;
using Microsoft.Extensions.Options;
using Eclipse.Common.Telegram;

namespace Eclipse.Application.Tests.Suggestions;

public class SuggestionServiceTests
{
    private readonly ISuggestionsService _sut;

    public SuggestionServiceTests()
    {
        var suggestionsSheetsService = Substitute.For<ISuggestionsSheetsService>();
        var suggestions = SuggestionsGenerator.Generate(5, 1);

        suggestionsSheetsService.GetAll().Returns(suggestions);

        var userRepository = Substitute.For<IIdentityUserService>();
        var users = IdentityUserDtoGenerator.GenerateSlim(1, 5);

        userRepository.GetAllAsync().Returns(users);

        var telegramService = Substitute.For<ITelegramService>();
        var options = Substitute.For<IOptions<TelegramOptions>>();

        _sut = new SuggestionsService(suggestionsSheetsService, userRepository, telegramService, options);
    }

    [Fact]
    public async Task GetWithUserInfo_WhenRequested_ThenSuggestionsWithUsersReturned()
    {
        var result = await _sut.GetWithUserInfo();
        result.All(r => r.User is not null && !r.Text.Contains(' ')).Should().BeTrue();
    }
}
