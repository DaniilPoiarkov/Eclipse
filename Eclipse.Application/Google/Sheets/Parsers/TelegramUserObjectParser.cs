using Eclipse.Application.Exceptions;
using Eclipse.Application.Extensions;
using Eclipse.Core.Models;
using Eclipse.Infrastructure.Google.Sheets;

namespace Eclipse.Application.Google.Sheets.Parsers;

public class TelegramUserObjectParser : IObjectParser<TelegramUser>
{
    public TelegramUser Parse(IList<object> values)
    {
        if (!CanBeParsed(values))
        {
            throw new UnableToParseValueException("objects", "TelegramUser");
        }

        var id = values[0].ToLong();
        var name = values[1].ToString();

        var userName = values.Count == 3
            ? values[2].ToString()
            : string.Empty;

        return new TelegramUser(id, name ?? string.Empty, userName ?? string.Empty);
    }

    public IList<object> Parse(TelegramUser value)
    {
        return new List<object> { value.Id, value.Name, value.Username ?? string.Empty };
    }

    private static bool CanBeParsed(IList<object> values)
    {
        return values.Count == 3 || values.Count == 2;
    }
}
