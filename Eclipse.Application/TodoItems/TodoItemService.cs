using Eclipse.Application.Contracts.Base;
using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.Exceptions;
using Eclipse.Application.TodoItems.Exceptions;
using Eclipse.Domain.TodoItems;

using FluentValidation;

namespace Eclipse.Application.TodoItems;

internal class TodoItemService : ITodoItemService
{
    private static readonly int _limit = 7;

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

    public async Task<TodoItemDto> CreateAsync(CreateTodoItemDto input, CancellationToken cancellationToken = default)
    {
        var result = _validator.Validate(input);

        if (!result.IsValid)
        {
            var errors = result.Errors.Select(e => e.ErrorMessage)
                .Distinct()
                .ToArray();

            throw new TodoItemValidationException(errors);
        }

        var userItems = await _todoItemRepository.GetByExpressionAsync(i => i.TelegramUserId == input.UserId, cancellationToken);

        if (userItems.Count == _limit)
        {
            throw new TodoItemLimitException(_limit);
        }

        var todoItem = new TodoItem(Guid.NewGuid(), input.UserId, input.Text!);

        await _todoItemRepository.CreateAsync(todoItem, cancellationToken);

        return _mapper.Map(todoItem);
    }

    public async Task FinishItemAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        var item = await _todoItemRepository.FindAsync(itemId, cancellationToken)
            ?? throw new ObjectNotFoundException(nameof(TodoItem));

        item.MarkAsFinished();

        await _todoItemRepository.DeleteAsync(itemId, cancellationToken);
    }

    public async Task<IReadOnlyList<TodoItemDto>> GetUserItemsAsync(long userId, CancellationToken cancellationToken = default)
    {
        var items = (await _todoItemRepository.GetByExpressionAsync(item => item.TelegramUserId == userId, cancellationToken))
            .Select(_mapper.Map)
            .ToList();

        return items;
    }
}
