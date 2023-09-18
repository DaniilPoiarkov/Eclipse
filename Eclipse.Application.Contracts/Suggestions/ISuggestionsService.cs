namespace Eclipse.Application.Contracts.Suggestions;

public interface ISuggestionsService
{
    IReadOnlyList<SuggestionAndUserDto> GetWithUserInfo();
}
