namespace Eclipse.Application.TodoItems.Exceptions;

public class TodoItemLimitException : Exception
{
    public TodoItemLimitException(int limit) : base(
        $"Not so fast cowboy😳{Environment.NewLine}" +
        $"You already have {limit} items. " +
        $"Don't you think it's time to finish at least one before planning something more? 🤔") { }
}
