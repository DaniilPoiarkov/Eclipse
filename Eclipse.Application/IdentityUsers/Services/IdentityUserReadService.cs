using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Common.Results;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Domain.Shared.Errors;

namespace Eclipse.Application.IdentityUsers.Services;

internal sealed class IdentityUserReadService : IIdentityUserReadService
{
    private readonly IdentityUserManager _userManager;

    private readonly IIdentityUserRepository _repository;

    public IdentityUserReadService(IdentityUserManager userManager, IIdentityUserRepository repository)
    {
        _userManager = userManager;
        _repository = repository;
    }

    public async Task<IReadOnlyList<IdentityUserSlimDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userManager.GetAllAsync(cancellationToken);

        return users
            .Select(u => u.ToSlimDto())
            .ToList();
    }

    public async Task<IReadOnlyList<IdentityUserSlimDto>> GetFilteredListAsync(GetUsersRequest request, CancellationToken cancellationToken = default)
    {
        var users = await _repository.GetByFilterAsync(request.Name, request.UserName, request.NotificationsEnabled, cancellationToken);

        return users
            .Select(u => u.ToSlimDto())
            .ToArray();
    }

    public async Task<Result<IdentityUserDto>> GetByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByChatIdAsync(chatId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(IdentityUser));
        }

        return user.ToDto();
    }

    public async Task<Result<IdentityUserDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(IdentityUser));
        }

        return user.ToDto();
    }
}
