using Eclipse.Common.Linq;
using Eclipse.Common.Results;

namespace Eclipse.Application.Contracts.IdentityUsers;

public interface IIdentityUserService : IIdentityUserReadService, IIdentityUserCreateUpdateService, IIdentityUserLogicService
{

}

public interface IIdentityUserReadService
{
    Task<IReadOnlyList<IdentityUserSlimDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<IdentityUserSlimDto>> GetFilteredListAsync(GetUsersRequest request, CancellationToken cancellationToken = default);

    Task<PaginatedList<IdentityUserSlimDto>> GetPaginatedListAsync(PaginationRequest<GetUsersRequest> request, CancellationToken cancellationToken = default);

    Task<Result<IdentityUserDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result<IdentityUserDto>> GetByChatIdAsync(long chatId, CancellationToken cancellationToken = default);
}

public interface IIdentityUserCreateUpdateService
{
    Task<Result<IdentityUserDto>> CreateAsync(IdentityUserCreateDto createDto, CancellationToken cancellationToken = default);

    Task<Result<IdentityUserDto>> UpdateAsync(Guid id, IdentityUserUpdateDto updateDto, CancellationToken cancellationToken = default);
}

public interface IIdentityUserLogicService
{
    Task<Result<IdentityUserDto>> SetUserGmtTimeAsync(Guid id, TimeOnly currentUserTime, CancellationToken cancellationToken = default);
}
