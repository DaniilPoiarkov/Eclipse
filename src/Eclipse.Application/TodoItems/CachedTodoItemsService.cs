﻿using Eclipse.Application.Caching;
using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Common.Results;

namespace Eclipse.Application.TodoItems;

internal sealed class CachedTodoItemsService : IdentityUserCachingFixture, ITodoItemService
{
    private readonly ITodoItemService _todoItemService;
    public CachedTodoItemsService(IIdentityUserCache userCache, ITodoItemService todoItemService) : base(userCache)
    {
        _todoItemService = todoItemService;
    }

    public Task<Result<IdentityUserDto>> CreateAsync(CreateTodoItemDto input, CancellationToken cancellationToken = default)
    {
        return WithCachingAsync(() => _todoItemService.CreateAsync(input, cancellationToken));
    }

    public Task<Result<IdentityUserDto>> FinishItemAsync(long chatId, Guid itemId, CancellationToken cancellationToken = default)
    {
        return WithCachingAsync(() => _todoItemService.FinishItemAsync(chatId, itemId, cancellationToken));
    }
}
