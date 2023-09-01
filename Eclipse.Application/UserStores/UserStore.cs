﻿using Eclipse.Application.Contracts.UserStores;
using Eclipse.Infrastructure.Cache;
using Eclipse.Infrastructure.Telegram;

namespace Eclipse.Application.UserStores;

internal class UserStore : IUserStore
{
    private readonly ICacheService _cacheService;

    private static readonly string Key = "Telegram-Users";

    public UserStore(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public void AddUser(TelegramUser user)
    {
        var key = new CacheKey(Key);

        var users = _cacheService.GetAndDelete<List<TelegramUser>>(key)
            ?? new List<TelegramUser>();

        if (!users.Exists(u => u.Id == user.Id))
        {
            users.Add(user);
        }

        _cacheService.Set(key, users);
    }

    public IReadOnlyList<TelegramUser> GetUsers()
    {
        var key = new CacheKey(Key);

        return _cacheService.Get<List<TelegramUser>>(key)
            ?? new List<TelegramUser>();
    }
}