using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.Exceptions;
using Eclipse.Application.Extensions;
using Eclipse.Infrastructure.Google.Sheets;

namespace Eclipse.Application.Google.Sheets.Parsers;

public class TodoItemObjectParser : IObjectParser<TodoItemDto>
{
    public TodoItemDto Parse(IList<object> values)
    {
        if (values.Count != 6)
        {
            throw new UnableToParseValueException(nameof(values), nameof(TodoItemDto));
        }

        var id = values[0].ToGuid();
        var userId = values[1].ToLong();
        var text = values[2].ToString();
        var isFinished = values[3].ToBool();
        var createdAt = values[4].ToDateTime();

        var item = new TodoItemDto
        {
            Id = id,
            TelegramUserId = userId,
            Text = text ?? string.Empty,
            IsFinished = isFinished,
            CreatedAt = createdAt,
        };

        if (item.IsFinished)
        {
            item.FinishedAt = values[5].ToDateTime();
        }

        return item;
    }

    public IList<object> Parse(TodoItemDto value)
    {
        return new List<object> { value.Id, value.TelegramUserId, value.Text, value.IsFinished, value.CreatedAt, value.FinishedAt?.ToString() ?? "null" };
    }
}
