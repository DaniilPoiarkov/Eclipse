using Eclipse.Application.Caching;
using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Results;

namespace Eclipse.Application.TodoItems;

internal sealed class CachedTodoItemsService : UserCachingFixture, ITodoItemService
{
    private readonly ITodoItemService _todoItemService;
    public CachedTodoItemsService(IUserCache userCache, ITodoItemService todoItemService) : base(userCache)
    {
        _todoItemService = todoItemService;
    }

    public Task<Result<UserDto>> CreateAsync(long chatId, CreateTodoItemDto input, CancellationToken cancellationToken = default)
    {
        return WithCachingAsync(() => _todoItemService.CreateAsync(chatId, input, cancellationToken), cancellationToken);
    }

    public Task<Result<UserDto>> FinishItemAsync(long chatId, Guid itemId, CancellationToken cancellationToken = default)
    {
        return WithCachingAsync(() => _todoItemService.FinishItemAsync(chatId, itemId, cancellationToken), cancellationToken);
    }

    public Task<Result<List<TodoItemDto>>> GetListAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _todoItemService.GetListAsync(userId, cancellationToken);
    }

    public Task<Result<TodoItemDto>> CreateAsync(Guid userId, CreateTodoItemDto model, CancellationToken cancellationToken = default)
    {
        return _todoItemService.CreateAsync(userId, model, cancellationToken);
    }

    public Task<Result<TodoItemDto>> GetAsync(Guid userId, Guid todoItemId, CancellationToken cancellationToken = default)
    {
        return _todoItemService.GetAsync(userId, todoItemId, cancellationToken);
    }
}
