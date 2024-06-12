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

    public CachedUserService(IUserCache userCache, IUserService userService) : base(userCache)
    {
        _userService = userService;
    }

    public Task<Result<UserDto>> CreateAsync(UserCreateDto createDto, CancellationToken cancellationToken = default)
    {
        return WithCachingAsync(() => _userService.CreateAsync(createDto, cancellationToken), cancellationToken);
    }

    public Task<IReadOnlyList<UserSlimDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _userService.GetAllAsync(cancellationToken);
    }

    public Task<PaginatedList<UserSlimDto>> GetListAsync(PaginationRequest<GetUsersRequest> request, CancellationToken cancellationToken = default)
    {
        return _userService.GetListAsync(request, cancellationToken);
    }

    public async Task<Result<UserDto>> GetByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var user = await UserCache.GetByChatIdAsync(chatId, cancellationToken);

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
