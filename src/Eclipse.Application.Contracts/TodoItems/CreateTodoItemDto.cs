namespace Eclipse.Application.Contracts.TodoItems;

[Serializable]
public sealed class CreateTodoItemDto
{
    public string? Text { get; set; }
}
