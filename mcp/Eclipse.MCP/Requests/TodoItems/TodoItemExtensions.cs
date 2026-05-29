using Eclipse.MCP.Core.Client;

namespace Eclipse.MCP.Requests.TodoItems;

internal static class TodoItemExtensions
{
    public static Task<EclipseResponse<TodoItemResponse[]>> GetTodoItemsAsync(this IEclipseClient client, CancellationToken cancellationToken = default)
        => client.SendRequestAsync(new GetTodoItemsRequest(), cancellationToken);

    public static Task<EclipseResponse<TodoItemResponse>> GetTodoItemAsync(this IEclipseClient client, Guid todoItemId, CancellationToken cancellationToken = default)
        => client.SendRequestAsync(new GetTodoItemRequest(todoItemId), cancellationToken);

    public static Task<EclipseResponse<TodoItemResponse>> CreateTodoItemAsync(this IEclipseClient client, string text, CancellationToken cancellationToken = default)
        => client.SendRequestAsync(new CreateTodoItemRequest(text), cancellationToken);

    public static Task<EclipseResponse<string>> FinishTodoItemAsync(this IEclipseClient client, Guid todoItemId, CancellationToken cancellationToken = default)
        => client.SendRequestAsync(new FinishTodoItemRequest(todoItemId), cancellationToken);
}
