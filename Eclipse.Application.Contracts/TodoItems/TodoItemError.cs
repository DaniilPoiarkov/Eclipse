namespace Eclipse.Application.Contracts.TodoItems;

public static class TodoItemError
{
    public static class Codes
    {
        private static readonly string _prefix = "TodoItem:";

        public static readonly string Null = $"{_prefix}Null";

        public static readonly string Empty = $"{_prefix}Empty";

        public static readonly string MinLength = $"{_prefix}MinLength";

        public static readonly string MaxLength = $"{_prefix}MaxLength";
    }

    public static class Messages
    {
        private static readonly string _prefix = "TodoItem:";

        public static readonly string Empty = $"{_prefix}Empty";

        public static readonly string MaxLength = $"{_prefix}MaxLength";
    }
}
