using Eclipse.Common.Exceptions;

namespace Eclipse.Domain.TodoItems;

[Serializable]
internal sealed class TodoItemValidationException : EclipseValidationException
{
    internal TodoItemValidationException(string error, params object[] args)
        : base(error)
    {
        for (int i = 0; i < args.Length; i++)
        {
            WithData($"{{{i}}}", args[i]);
        }
    }
}
