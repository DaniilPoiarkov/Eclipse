using Eclipse.Application.Contracts.Base;
using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.Exceptions;
using Eclipse.DataAccess.TodoItems;
using Eclipse.Domain.TodoItems;

using FluentValidation;

namespace Eclipse.Application.TodoItems;

internal class TodoItemService : ITodoItemService
{
    private readonly ITodoItemRepository _todoItemRepository;

    private readonly IValidator<CreateTodoItemDto> _validator;

    private readonly IMapper<TodoItem, TodoItemDto> _mapper;

    public TodoItemService(
        ITodoItemRepository todoItemRepository,
        IValidator<CreateTodoItemDto> validator,
        IMapper<TodoItem, TodoItemDto> mapper)
    {
        _todoItemRepository = todoItemRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public TodoItemDto AddItem(CreateTodoItemDto input)
    {
        _validator.ValidateAndThrow(input);

        var todoItem = new TodoItem(Guid.NewGuid(), input.UserId, input.Text!);

        _todoItemRepository.Add(todoItem);

        return _mapper.Map(todoItem);
    }

    public void FinishItem(Guid itemId)
    {
        var item = _todoItemRepository.GetById(itemId)
            ?? throw new ObjectNotFoundException(nameof(TodoItem));

        item.MarkAsFinished();

        _todoItemRepository.Update(item);
    }

    public IReadOnlyList<TodoItemDto> GetUserItems(long userId)
    {
        var items = _todoItemRepository.GetAll()
            .Where(item => item.TelegramUserId == userId)
            .Select(_mapper.Map)
            .ToList();

        return items;
    }
}
