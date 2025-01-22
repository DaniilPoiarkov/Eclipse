using Eclipse.Application.Reminders.Core;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Reminders.FinishTodoItems;

internal sealed class FinishTodoItemsOptionsConvertor : IOptionsConvertor<User, FinishTodoItemsSchedulerOptions>
{
    public FinishTodoItemsSchedulerOptions Convert(User from)
    {
        return new FinishTodoItemsSchedulerOptions(from.Id, from.Gmt);
    }
}
