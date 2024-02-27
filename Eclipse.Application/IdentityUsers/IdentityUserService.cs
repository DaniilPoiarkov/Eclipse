using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Common.Linq;
using Eclipse.Common.Results;

namespace Eclipse.Application.IdentityUsers;

internal sealed class IdentityUserService : IIdentityUserService
{
    private readonly IIdentityUserCreateUpdateService _createUpdateService;

    private readonly IIdentityUserLogicService _logicService;

    private readonly IIdentityUserReadService _readService;

    public IdentityUserService(IIdentityUserCreateUpdateService createUpdateService, IIdentityUserLogicService logicService, IIdentityUserReadService readService)
    {
        _createUpdateService = createUpdateService;
        _logicService = logicService;
        _readService = readService;
    }

    public Task<Result<IdentityUserDto>> CreateAsync(IdentityUserCreateDto createDto, CancellationToken cancellationToken = default)
    {
        return _createUpdateService.CreateAsync(createDto, cancellationToken);
    }

    public Task<IReadOnlyList<IdentityUserSlimDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _readService.GetAllAsync(cancellationToken);
    }

    public Task<IReadOnlyList<IdentityUserSlimDto>> GetFilteredListAsync(GetUsersRequest request, CancellationToken cancellationToken = default)
    {
        return _readService.GetFilteredListAsync(request, cancellationToken);
    }

    public Task<Result<IdentityUserDto>> GetByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        return _readService.GetByChatIdAsync(chatId, cancellationToken);
    }

    public Task<PaginatedList<IdentityUserSlimDto>> GetPaginatedListAsync(PaginationRequest<GetUsersRequest> request, CancellationToken cancellationToken = default)
    {
        return _readService.GetPaginatedListAsync(request, cancellationToken);
    }

    public Task<Result<IdentityUserDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _readService.GetByIdAsync(id, cancellationToken);
    }

    public Task<Result<IdentityUserDto>> SetUserGmtTimeAsync(Guid id, TimeOnly currentUserTime, CancellationToken cancellationToken = default)
    {
        return _logicService.SetUserGmtTimeAsync(id, currentUserTime, cancellationToken);
    }

    public Task<Result<IdentityUserDto>> UpdateAsync(Guid id, IdentityUserUpdateDto updateDto, CancellationToken cancellationToken = default)
    {
        return _createUpdateService.UpdateAsync(id, updateDto, cancellationToken);
    }
}
