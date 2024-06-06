using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Results;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Users.Services;

internal sealed class UserLogicService : IUserLogicService
{
    private readonly UserManager _userManager;

    public UserLogicService(UserManager userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<UserDto>> SetUserGmtTimeAsync(Guid id, TimeOnly currentUserTime, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(User));
        }

        user.SetGmt(currentUserTime);

        await _userManager.UpdateAsync(user, cancellationToken);

        return user.ToDto();
    }
}
