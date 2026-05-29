using Eclipse.MCP.Core.Client;

using System.Net.Http.Json;

namespace Eclipse.MCP.Requests.TodoItems;

internal sealed class CreateTodoItemRequest : IRequest<TodoItemResponse>
{
    private readonly string _text;

    public CreateTodoItemRequest(string text)
    {
        _text = text;
    }

    public HttpRequestMessage Build()
    {
        var message = new HttpRequestMessage(HttpMethod.Post, "/api/todo-items/add");
        message.Content = JsonContent.Create(new { text = _text });
        return message;
    }

    public async ValueTask<TodoItemResponse> ParseAsync(HttpContent content, CancellationToken cancellationToken = default)
        => await content.ReadFromJsonAsync<TodoItemResponse>(cancellationToken) ?? new TodoItemResponse();
}
