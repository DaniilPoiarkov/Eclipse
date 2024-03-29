﻿using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Common.Cache;

namespace Eclipse.Application.IdentityUsers;

internal sealed class IdentityUserCache : IIdentityUserCache
{
    private readonly ICacheService _cacheService;

    private static readonly CacheKey Key = new("Identity-Users");

    public IdentityUserCache(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public void AddOrUpdate(IdentityUserDto user)
    {
        var users = GetList();

        var existing = users.FirstOrDefault(u => u.Id == user.Id);

        if (existing is not null)
        {
            users.Remove(existing);
        }

        users.Add(user);

        _cacheService.Set(Key, users);
    }

    public IdentityUserDto? GetByChatId(long chatId)
    {
        return GetList().FirstOrDefault(u => u.ChatId == chatId);
    }

    public IdentityUserDto? GetById(Guid userId)
    {
        return GetList().FirstOrDefault(u => u.Id == userId);
    }

    public IReadOnlyList<IdentityUserDto> GetAll() => GetList().AsReadOnly();

    private List<IdentityUserDto> GetList()
    {
        return _cacheService.Get<List<IdentityUserDto>>(Key) ?? [];
    }
}
