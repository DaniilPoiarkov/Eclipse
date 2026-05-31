using Eclipse.MCP.Core.Client;

using System.Net.Http.Json;

namespace Eclipse.MCP.Requests.TodoItems;

internal sealed class GetTodoItemRequest : IRequest<TodoItemResponse>
{
    private readonly Guid _todoItemId;

    public GetTodoItemRequest(Guid todoItemId)
    {
        _todoItemId = todoItemId;
    }

    public HttpRequestMessage Build() => new(HttpMethod.Get, $"/api/todo-items/{_todoItemId}");

    public async ValueTask<TodoItemResponse> ParseAsync(HttpContent content, CancellationToken cancellationToken = default)
        => await content.ReadFromJsonAsync<TodoItemResponse>(cancellationToken) ?? new TodoItemResponse();
}
