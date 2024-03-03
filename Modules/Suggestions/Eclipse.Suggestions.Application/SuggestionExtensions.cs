using Eclipse.Suggestions.Application.Contracts;
using Eclipse.Suggestions.Domain;

namespace Eclipse.Suggestions.Application;

internal static class SuggestionExtensions
{
    internal static SuggestionModel ToDto(this Suggestion suggestion)
    {
        return new SuggestionModel
        {
            Id = suggestion.Id,
            Text = suggestion.Text,
            ChatId = suggestion.ChatId,
            CreatedAt = suggestion.CreatedAt,
        };
    }
}
