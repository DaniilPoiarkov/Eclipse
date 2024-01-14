using Eclipse.Application.Contracts.Suggestions;
using Eclipse.Domain.Suggestions;
using Eclipse.Infrastructure.Google.Sheets;

namespace Eclipse.Application.Google.Sheets.Parsers;

public class SuggestionObjectParser : IObjectParser<SuggestionDto>
{
    public SuggestionDto Parse(IList<object> values)
    {
        if (values.Count != 4)
        {
            throw new UnableToParseValueException(nameof(values), nameof(Suggestion));
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
        return new List<object> { value.Id, value.Text, value.TelegramUserId, value.CreatedAt };
    }
}
