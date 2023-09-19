using Eclipse.Domain.Shared.TodoItems;

namespace Eclipse.Application.Contracts.TodoItems;

public static class TodoItemError
{
    public static class Codes
    {
        private static readonly string _prefix = "Todo:";

        public static readonly string Null = $"{_prefix}Null";

        public static readonly string Empty = $"{_prefix}Empty";

        public static readonly string MinLength = $"{_prefix}MinLength";

        public static readonly string MaxLength = $"{_prefix}MaxLength";
    }

    public static class Messages
    {
        public static readonly string Empty = "To do item cannot be empty";

        public static readonly string MaxLength = $"To do item text cant be more than {TodoItemConstants.MaxLength} characters";
    }
}
