using Eclipse.Common.Linq;
using Eclipse.Common.Results;

namespace Eclipse.Application.Contracts.Users;

public interface IUserService : IUserReadService, IUserCreateUpdateService, IUserLogicService
{

}

public interface IUserReadService
{
    Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<PaginatedList<UserSlimDto>> GetListAsync(PaginationRequest<GetUsersRequest> request, CancellationToken cancellationToken = default);

    Task<Result<UserDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result<UserDto>> GetByChatIdAsync(long chatId, CancellationToken cancellationToken = default);
}

public interface IUserCreateUpdateService
{
    Task<Result<UserDto>> CreateAsync(UserCreateDto model, CancellationToken cancellationToken = default);

    Task<Result<UserDto>> UpdateAsync(Guid id, UserUpdateDto model, CancellationToken cancellationToken = default);

    Task<Result<UserDto>> UpdatePartialAsync(Guid id, UserPartialUpdateDto model, CancellationToken cancellationToken = default);
}

public interface IUserLogicService
{
    Task<Result<UserDto>> SetUserGmtTimeAsync(Guid id, TimeOnly currentUserTime, CancellationToken cancellationToken = default);
}
