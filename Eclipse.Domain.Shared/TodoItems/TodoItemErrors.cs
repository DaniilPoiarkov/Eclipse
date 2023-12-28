namespace Eclipse.Domain.Shared.TodoItems;

public static class TodoItemErrors
{
    private static readonly string _prefix = "TodoItem";

    public static class Messages
    {
        public static readonly string AlreadyFinished = $"{_prefix}:{nameof(AlreadyFinished)}";

        public static readonly string Empty = $"{_prefix}:{nameof(Empty)}";

        public static readonly string MaxLength = $"{_prefix}:{nameof(MaxLength)}";
    }
}
