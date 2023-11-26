namespace Eclipse.Domain.Shared.TodoItems;

public static class TodoItemErrors
{
    private static readonly string _prefix = "TodoItem";

    public static class Messages
    {
        public static readonly string AlreadyFinished = $"{_prefix}:{nameof(AlreadyFinished)}";

        public static readonly string Empty = $"{_prefix}:Empty";

        public static readonly string MaxLength = $"{_prefix}:MaxLength";
    }

    public static class Codes
    {
        public static readonly string Null = $"{_prefix}:Null";

        public static readonly string Empty = $"{_prefix}:Empty";

        public static readonly string MinLength = $"{_prefix}:MinLength";

        public static readonly string MaxLength = $"{_prefix}:MaxLength";
    }
}
