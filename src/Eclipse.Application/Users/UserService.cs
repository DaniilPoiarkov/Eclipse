using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Linq;
using Eclipse.Common.Results;

namespace Eclipse.Application.Users;

internal sealed class UserService : IUserService
{
    private readonly IUserCreateUpdateService _createUpdateService;

    private readonly IUserLogicService _logicService;

    private readonly IUserReadService _readService;

    public UserService(IUserCreateUpdateService createUpdateService, IUserLogicService logicService, IUserReadService readService)
    {
        _createUpdateService = createUpdateService;
        _logicService = logicService;
        _readService = readService;
    }

    public Task<Result<UserDto>> CreateAsync(UserCreateDto createDto, CancellationToken cancellationToken = default)
    {
        return _createUpdateService.CreateAsync(createDto, cancellationToken);
    }

    public Task<IReadOnlyList<UserSlimDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _readService.GetAllAsync(cancellationToken);
    }

    public Task<IReadOnlyList<UserSlimDto>> GetFilteredListAsync(GetUsersRequest request, CancellationToken cancellationToken = default)
    {
        return _readService.GetFilteredListAsync(request, cancellationToken);
    }

    public Task<Result<UserDto>> GetByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        return _readService.GetByChatIdAsync(chatId, cancellationToken);
    }

    public Task<PaginatedList<UserSlimDto>> GetPaginatedListAsync(PaginationRequest<GetUsersRequest> request, CancellationToken cancellationToken = default)
    {
        return _readService.GetPaginatedListAsync(request, cancellationToken);
    }

    public Task<Result<UserDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _readService.GetByIdAsync(id, cancellationToken);
    }

    public Task<Result<UserDto>> SetUserGmtTimeAsync(Guid id, TimeOnly currentUserTime, CancellationToken cancellationToken = default)
    {
        return _logicService.SetUserGmtTimeAsync(id, currentUserTime, cancellationToken);
    }

    public Task<Result<UserDto>> UpdateAsync(Guid id, UserUpdateDto updateDto, CancellationToken cancellationToken = default)
    {
        return _createUpdateService.UpdateAsync(id, updateDto, cancellationToken);
    }
}
