using Eclipse.MCP.Core.Client;
using Eclipse.MCP.Core.Tools;
using Eclipse.MCP.Requests.TodoItems;

using ModelContextProtocol.Server;

using System.ComponentModel;

namespace Eclipse.MCP.Tools;

public sealed class TodoItemTools
{
    private readonly IEclipseClient _client;

    public TodoItemTools(IEclipseClient client)
    {
        _client = client;
    }

    [McpServerTool(Name = "eclipse_todo_items_get_list")]
    [Description("Retrieves all todo items for the authenticated user.\n" +
        "Use when the user asks to see, list, or show their todo items or tasks.\n" +
        "Below are user input samples which show when to call this tool:\n" +
        "<sample1>Show me my todo items</sample1>\n" +
        "<sample2>What tasks do I have?</sample2>\n" +
        "<sample3>List all my todos</sample3>\n")
    ]
    public async Task<ToolResponse<TodoItemResponse[]>> GetTodoItemsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.GetTodoItemsAsync(cancellationToken);
        return response.ToToolResponse();
    }

    [McpServerTool(Name = "eclipse_todo_items_get")]
    [Description("Retrieves a specific todo item by its ID.\n" +
        "Use when the user asks about a specific todo item and provides an ID.\n" +
        "Below are user input samples which show when to call this tool:\n" +
        "<sample1>Get todo item with id 123</sample1>\n" +
        "<sample2>Show me the details of todo item 456</sample2>\n")
    ]
    public async Task<ToolResponse<TodoItemResponse>> GetTodoItemAsync(
        [Description("The unique identifier of the todo item.")] Guid todoItemId,
        CancellationToken cancellationToken = default)
    {
        var response = await _client.GetTodoItemAsync(todoItemId, cancellationToken);
        return response.ToToolResponse();
    }

    [McpServerTool(Name = "eclipse_todo_items_add")]
    [Description("Creates a new todo item for the authenticated user.\n" +
        "Use when the user wants to add, create, or set a new task or todo item.\n" +
        "Below are user input samples which show when to call this tool:\n" +
        "<sample1>Add a todo item: buy groceries</sample1>\n" +
        "<sample2>Create a new task to call the dentist</sample2>\n" +
        "<sample3>Add task: finish the report</sample3>\n")
    ]
    public async Task<ToolResponse<TodoItemResponse>> AddTodoItemAsync(
        [Description("The text content of the todo item.")] string text,
        CancellationToken cancellationToken = default)
    {
        var response = await _client.CreateTodoItemAsync(text, cancellationToken);
        return response.ToToolResponse();
    }

    [McpServerTool(Name = "eclipse_todo_items_finish")]
    [Description("Marks a todo item as finished.\n" +
        "Use when the user wants to complete, finish, or mark a todo item as done.\n" +
        "Below are user input samples which show when to call this tool:\n" +
        "<sample1>Finish todo item 123</sample1>\n" +
        "<sample2>Mark task 456 as done</sample2>\n" +
        "<sample3>Complete the todo item with id 789</sample3>\n")
    ]
    public async Task<ToolResponse<string>> FinishTodoItemAsync(
        [Description("The unique identifier of the todo item to finish.")] Guid todoItemId,
        CancellationToken cancellationToken = default)
    {
        var response = await _client.FinishTodoItemAsync(todoItemId, cancellationToken);
        return response.ToToolResponse();
    }
}
