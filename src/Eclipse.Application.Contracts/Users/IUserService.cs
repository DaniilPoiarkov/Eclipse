using Eclipse.Common.Linq;
using Eclipse.Common.Results;

namespace Eclipse.Application.Contracts.Users;

public interface IUserService : IUserReadService, IUserCreateUpdateService, IUserLogicService
{

}

public interface IUserReadService
{
    Task<IReadOnlyList<UserSlimDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UserSlimDto>> GetFilteredListAsync(GetUsersRequest request, CancellationToken cancellationToken = default);

    Task<PaginatedList<UserSlimDto>> GetPaginatedListAsync(PaginationRequest<GetUsersRequest> request, CancellationToken cancellationToken = default);

    Task<Result<UserDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result<UserDto>> GetByChatIdAsync(long chatId, CancellationToken cancellationToken = default);
}

public interface IUserCreateUpdateService
{
    Task<Result<UserDto>> CreateAsync(UserCreateDto createDto, CancellationToken cancellationToken = default);

    Task<Result<UserDto>> UpdateAsync(Guid id, UserUpdateDto updateDto, CancellationToken cancellationToken = default);
}

public interface IUserLogicService
{
    Task<Result<UserDto>> SetUserGmtTimeAsync(Guid id, TimeOnly currentUserTime, CancellationToken cancellationToken = default);
}
