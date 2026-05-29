using Eclipse.MCP.Core.Client;

namespace Eclipse.MCP.Requests.TodoItems;

internal sealed class FinishTodoItemRequest : IRequest<string>
{
    private readonly Guid _todoItemId;

    public FinishTodoItemRequest(Guid todoItemId)
    {
        _todoItemId = todoItemId;
    }

    public HttpRequestMessage Build() => new(HttpMethod.Post, $"/api/todo-items/{_todoItemId}/finish");

    public ValueTask<string> ParseAsync(HttpContent content, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(string.Empty);
}
