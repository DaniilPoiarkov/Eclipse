namespace Eclipse.Application.TodoItems.Exceptions;

internal class TodoItemValidationException : TodoItemException
{
    public TodoItemValidationException(params string[] args) : base("Eclipse:ValidationFailed", args)
    {
    }
}
