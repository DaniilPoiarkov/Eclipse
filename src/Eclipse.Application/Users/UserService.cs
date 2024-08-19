using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Linq;
using Eclipse.Common.Results;

namespace Eclipse.Application.Users;

internal sealed class UserService : IUserService
{
    private readonly IUserCreateUpdateService _createUpdateService;

    private readonly IUserReadService _readService;

    public UserService(IUserCreateUpdateService createUpdateService, IUserReadService readService)
    {
        _createUpdateService = createUpdateService;
        _readService = readService;
    }

    public Task<Result<UserDto>> CreateAsync(UserCreateDto model, CancellationToken cancellationToken = default)
    {
        return _createUpdateService.CreateAsync(model, cancellationToken);
    }

    public Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _readService.GetAllAsync(cancellationToken);
    }

    public Task<Result<UserDto>> GetByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        return _readService.GetByChatIdAsync(chatId, cancellationToken);
    }

    public Task<PaginatedList<UserSlimDto>> GetListAsync(PaginationRequest<GetUsersRequest> request, CancellationToken cancellationToken = default)
    {
        return _readService.GetListAsync(request, cancellationToken);
    }

    public Task<Result<UserDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _readService.GetByIdAsync(id, cancellationToken);
    }

    public Task<Result<UserDto>> UpdateAsync(Guid id, UserUpdateDto model, CancellationToken cancellationToken = default)
    {
        return _createUpdateService.UpdateAsync(id, model, cancellationToken);
    }

    public Task<Result<UserDto>> UpdatePartialAsync(Guid id, UserPartialUpdateDto model, CancellationToken cancellationToken = default)
    {
        return _createUpdateService.UpdatePartialAsync(id, model, cancellationToken);
    }
}
