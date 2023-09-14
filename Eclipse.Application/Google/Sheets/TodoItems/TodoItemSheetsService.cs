using Eclipse.Application.Contracts.Google.Sheets.TodoItems;
using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Infrastructure.Google.Sheets;

using Microsoft.Extensions.Configuration;

namespace Eclipse.Application.Google.Sheets.TodoItems;

internal class TodoItemSheetsService : EclipseSheetsService<TodoItemDto>, ITodoItemSheetsService
{
    public TodoItemSheetsService(
        IGoogleSheetsService service,
        IObjectParser<TodoItemDto> parser,
        IConfiguration configuration)
        : base(service, parser, configuration)
    {
    }

    protected override string Range => Configuration["Sheets:TodoItemsRange"]!;
}
