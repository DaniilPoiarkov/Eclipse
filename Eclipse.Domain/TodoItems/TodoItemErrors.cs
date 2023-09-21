namespace Eclipse.Domain.TodoItems;

internal static class TodoItemErrors
{
    private static readonly string _prefix = "TodoItem";

    internal static class Messages
    {
        public static readonly string AlreadyFinished = $"{_prefix}:{nameof(AlreadyFinished)}";
    }
}
