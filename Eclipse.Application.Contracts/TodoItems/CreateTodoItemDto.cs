namespace Eclipse.Application.Contracts.TodoItems;

public class CreateTodoItemDto
{
    public long UserId { get; set; }

    public string? Text { get; set; }
}
