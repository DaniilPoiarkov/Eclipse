using Eclipse.Application.Contracts.Suggestions;
using Eclipse.Common.Results;
using Eclipse.Common.Sheets;
using Eclipse.Domain.Suggestions;

namespace Eclipse.Application.Google.Sheets.Parsers;

public sealed class SuggestionObjectParser : IObjectParser<SuggestionDto>
{
    public Result<SuggestionDto> Parse(IList<object> values)
    {
        if (values.Count != 4)
        {
            return Error.Failure("Sheets.Parse", $"Unable to parse {nameof(values)} to {nameof(Suggestion)}");
        }

        var id = values[0].ToGuid();
        var text = values[1].ToString();
        var chatId = values[2].ToLong();
        var createdAt = values[3].ToDateTime();

        return new SuggestionDto
        {
            Id = id,
            Text = text ?? string.Empty,
            CreatedAt = createdAt,
            TelegramUserId = chatId,
        };
    }

    public IList<object> Parse(SuggestionDto value)
    {
        return [value.Id, value.Text, value.TelegramUserId, value.CreatedAt];
    }
}
