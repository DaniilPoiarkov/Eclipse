﻿using Eclipse.Common.Exceptions;

namespace Eclipse.Domain.TodoItems;

[Serializable]
internal sealed class TodoItemLimitException : EclipseValidationException
{
    internal TodoItemLimitException(int limit)
        : base("TodoItem:Limit")
    {
        WithData("{0}", limit);
    }
}
