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

    public Task<Result<UserDto>> CreateAsync(CreateTodoItemDto input, CancellationToken cancellationToken = default)
    {
        return WithCachingAsync(() => _todoItemService.CreateAsync(input, cancellationToken), cancellationToken);
    }

    public Task<Result<UserDto>> FinishItemAsync(long chatId, Guid itemId, CancellationToken cancellationToken = default)
    {
        return WithCachingAsync(() => _todoItemService.FinishItemAsync(chatId, itemId, cancellationToken), cancellationToken);
    }
}
