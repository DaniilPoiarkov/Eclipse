using Eclipse.Application.Contracts.Base;
using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.Exceptions;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Domain.TodoItems;

namespace Eclipse.Application.TodoItems;

internal class TodoItemService : ITodoItemService
{
    private readonly ITodoItemRepository _todoItemRepository;

    private readonly IdentityUserManager _userManager;

    private readonly IMapper<TodoItem, TodoItemDto> _mapper;

    public TodoItemService(
        ITodoItemRepository todoItemRepository,
        IdentityUserManager userManager,
        IMapper<TodoItem, TodoItemDto> mapper)
    {
        _todoItemRepository = todoItemRepository;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<TodoItemDto> CreateAsync(CreateTodoItemDto input, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByChatIdAsync(input.UserId, cancellationToken)
            ?? throw new ObjectNotFoundException(nameof(IdentityUser));

        var todoItem = user.AddTodoItem(input.Text);

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
