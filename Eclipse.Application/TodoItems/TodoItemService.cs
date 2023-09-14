using Eclipse.Application.Contracts.Google.Sheets.TodoItems;
using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.Exceptions;
using Eclipse.Domain.TodoItems;
using FluentValidation;

namespace Eclipse.Application.TodoItems;

internal class TodoItemService : ITodoItemService
{
    private readonly ITodoItemSheetsService _chronicleItemSheetsService;

    private readonly IValidator<TodoItemDto> _validator;

    public TodoItemService(ITodoItemSheetsService chronicleItemSheetsService, IValidator<TodoItemDto> validator)
    {
        _chronicleItemSheetsService = chronicleItemSheetsService;
        _validator = validator;
    }

    public TodoItemDto AddItem(long userId, string text)
    {
        var chronicleItem = new TodoItem(Guid.NewGuid(), userId, text);

        var dto = new TodoItemDto
        {
            Id = chronicleItem.Id,
            TelegramUserId = chronicleItem.TelegramUserId,
            Text = chronicleItem.Text,
            CreatedAt = chronicleItem.CreatedAt,
            IsFinished = chronicleItem.IsFinished,
        };

        _validator.ValidateAndThrow(dto);
        _chronicleItemSheetsService.Add(dto);

        return dto;
    }

    public void FinishItem(Guid itemId)
    {
        var item = _chronicleItemSheetsService.GetAll().FirstOrDefault(i => i.Id == itemId)
            ?? throw new ObjectNotFoundException(nameof(TodoItem));

        item.IsFinished = true;


    }

    public IReadOnlyList<TodoItemDto> GetUserItems(long userId)
    {
        throw new NotImplementedException();
    }
}
