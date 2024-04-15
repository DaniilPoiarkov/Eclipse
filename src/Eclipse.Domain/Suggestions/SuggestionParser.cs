using Eclipse.Common.Results;
using Eclipse.Common.Sheets;

namespace Eclipse.Domain.Suggestions;

internal sealed class SuggestionParser : IObjectParser<Suggestion>
{
    public Result<Suggestion> Parse(IList<object> values)
    {
        if (values.Count != 4)
        {
            return Error.Failure("Sheets.Parse", $"Unable to parse {nameof(values)} to {nameof(Suggestion)}");
        }

        var id = values[0].ToGuid();
        var text = values[1].ToString() ?? string.Empty;
        var chatId = values[2].ToLong();
        var createdAt = values[3].ToDateTime();

        return new Suggestion(id, text, chatId, createdAt);
    }

    public IList<object> Parse(Suggestion value)
    {
        return [value.Id, value.Text, value.TelegramUserId, value.CreatedAt];
    }
}
