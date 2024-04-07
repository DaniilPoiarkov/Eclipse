using Eclipse.Application.Caching;
using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Common.Linq;
using Eclipse.Common.Results;

namespace Eclipse.Application.IdentityUsers;

internal sealed class CachedIdentityUserService : IdentityUserCachingFixture, IIdentityUserService
{
    private readonly IIdentityUserService _identityUserService;

    public CachedIdentityUserService(IIdentityUserCache userCache, IIdentityUserService identityUserService) : base(userCache)
    {
        _identityUserService = identityUserService;
    }

    public Task<Result<IdentityUserDto>> CreateAsync(IdentityUserCreateDto createDto, CancellationToken cancellationToken = default)
    {
        return WithCachingAsync(() => _identityUserService.CreateAsync(createDto, cancellationToken), cancellationToken);
    }

    public Task<IReadOnlyList<IdentityUserSlimDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _identityUserService.GetAllAsync(cancellationToken);
    }

    public Task<IReadOnlyList<IdentityUserSlimDto>> GetFilteredListAsync(GetUsersRequest request, CancellationToken cancellationToken = default)
    {
        return _identityUserService.GetFilteredListAsync(request, cancellationToken);
    }

    public Task<PaginatedList<IdentityUserSlimDto>> GetPaginatedListAsync(PaginationRequest<GetUsersRequest> request, CancellationToken cancellationToken = default)
    {
        return _identityUserService.GetPaginatedListAsync(request, cancellationToken);
    }

    public async Task<Result<IdentityUserDto>> GetByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var user = await UserCache.GetByChatIdAsync(chatId, cancellationToken);

        if (user is not null)
        {
            return Result<IdentityUserDto>.Success(user);
        }

        return await WithCachingAsync(() => _identityUserService.GetByChatIdAsync(chatId, cancellationToken), cancellationToken);
    }

    public async Task<Result<IdentityUserDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await UserCache.GetByIdAsync(id, cancellationToken);

        if (user is not null)
        {
            return Result<IdentityUserDto>.Success(user);
        }

        return await WithCachingAsync(() => _identityUserService.GetByIdAsync(id, cancellationToken), cancellationToken);
    }

    public Task<Result<IdentityUserDto>> SetUserGmtTimeAsync(Guid userId, TimeOnly currentUserTime, CancellationToken cancellationToken = default)
    {
        return WithCachingAsync(() => _identityUserService.SetUserGmtTimeAsync(userId, currentUserTime, cancellationToken), cancellationToken);
    }

    public Task<Result<IdentityUserDto>> UpdateAsync(Guid id, IdentityUserUpdateDto updateDto, CancellationToken cancellationToken = default)
    {
        return WithCachingAsync(() => _identityUserService.UpdateAsync(id, updateDto, cancellationToken), cancellationToken);
    }
}
