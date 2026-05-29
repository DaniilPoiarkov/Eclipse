using Eclipse.MCP.Core.Client;

using System.Net.Http.Json;

namespace Eclipse.MCP.Requests.TodoItems;

internal sealed class GetTodoItemsRequest : IRequest<TodoItemResponse[]>
{
    public HttpRequestMessage Build() => new(HttpMethod.Get, "/api/todo-items");

    public async ValueTask<TodoItemResponse[]> ParseAsync(HttpContent content, CancellationToken cancellationToken = default)
        => await content.ReadFromJsonAsync<TodoItemResponse[]>(cancellationToken) ?? [];
}
