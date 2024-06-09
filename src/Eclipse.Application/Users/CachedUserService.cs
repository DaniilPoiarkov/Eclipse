using Eclipse.Application.Caching;
using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Cache;
using Eclipse.Common.Linq;
using Eclipse.Common.Results;

using Newtonsoft.Json;

namespace Eclipse.Application.Users;

internal sealed class CachedUserService : UserCachingFixture, IUserService
{
    private readonly IUserService _userService;

    private readonly ICacheService _cacheService;

    public CachedUserService(IUserCache userCache, IUserService userService, ICacheService cacheService) : base(userCache)
    {
        _userService = userService;
        _cacheService = cacheService;
    }

    public async Task<Result<UserDto>> CreateAsync(UserCreateDto createDto, CancellationToken cancellationToken = default)
    {
        await _cacheService.DeleteAsync("users-all", cancellationToken);
        var result = await _userService.CreateAsync(createDto, cancellationToken);

        await _cacheService.SetAsync($"users-id-{result.Value.Id}", result, CacheConsts.OneDay, cancellationToken);
        await _cacheService.SetAsync($"users-chat-id-{result.Value.ChatId}", result, CacheConsts.OneDay, cancellationToken);

        return result;
        
        return await WithCachingAsync(() => _userService.CreateAsync(createDto, cancellationToken), cancellationToken);
    }

    public Task<IReadOnlyList<UserSlimDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _cacheService.GetOrCreateAsync("users-all", () => _userService.GetAllAsync(cancellationToken), CacheConsts.OneDay, cancellationToken);
    }

    public Task<PaginatedList<UserSlimDto>> GetListAsync(PaginationRequest<GetUsersRequest> request, CancellationToken cancellationToken = default)
    {
        return _cacheService.GetOrCreateAsync(
            JsonConvert.SerializeObject(request),
            () => _userService.GetListAsync(request, cancellationToken),
            CacheConsts.FiveMinutes,
            cancellationToken
        );
    }

    public async Task<Result<UserDto>> GetByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var user = await _cacheService.GetAsync<UserDto>("users-chat-id", cancellationToken);// await UserCache.GetByChatIdAsync(chatId, cancellationToken);

        if (user is not null)
        {
            return Result<UserDto>.Success(user);
        }

        return await WithCachingAsync(() => _userService.GetByChatIdAsync(chatId, cancellationToken), cancellationToken);
    }

    public async Task<Result<UserDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await UserCache.GetByIdAsync(id, cancellationToken);

        if (user is not null)
        {
            return Result<UserDto>.Success(user);
        }

        return await WithCachingAsync(() => _userService.GetByIdAsync(id, cancellationToken), cancellationToken);
    }

    public Task<Result<UserDto>> SetUserGmtTimeAsync(Guid userId, TimeOnly currentUserTime, CancellationToken cancellationToken = default)
    {
        return WithCachingAsync(() => _userService.SetUserGmtTimeAsync(userId, currentUserTime, cancellationToken), cancellationToken);
    }

    public Task<Result<UserDto>> UpdateAsync(Guid id, UserUpdateDto updateDto, CancellationToken cancellationToken = default)
    {
        return WithCachingAsync(() => _userService.UpdateAsync(id, updateDto, cancellationToken), cancellationToken);
    }
}
