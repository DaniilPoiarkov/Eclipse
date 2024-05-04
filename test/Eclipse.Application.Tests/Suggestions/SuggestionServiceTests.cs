using Eclipse.Application.Contracts.Google.Sheets;
using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Suggestions;
using Eclipse.Domain.Suggestions;
using Eclipse.Tests.Generators;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Suggestions;

public class SuggestionServiceTests
{
    private readonly SuggestionsService _sut;

    public SuggestionServiceTests()
    {
        var suggestionsSheetsService = Substitute.For<IEclipseSheetsService<Suggestion>>();
        var suggestions = SuggestionsGenerator.Generate(5, 1);

        suggestionsSheetsService.GetAllAsync().Returns(suggestions);

        var userRepository = Substitute.For<IIdentityUserService>();
        var users = IdentityUserDtoGenerator.GenerateSlim(1, 5);

        userRepository.GetAllAsync().Returns(users);

        _sut = new SuggestionsService(suggestionsSheetsService, userRepository);
    }

    [Fact]
    public async Task GetWithUserInfo_WhenRequested_ThenSuggestionsWithUsersReturned()
    {
        var result = await _sut.GetWithUserInfo();
        result.All(r => r.User is not null && !r.Text.Contains(' ')).Should().BeTrue();
    }
}
