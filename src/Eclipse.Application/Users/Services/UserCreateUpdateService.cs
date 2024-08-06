using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Results;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Users.Services;

internal sealed class UserCreateUpdateService : IUserCreateUpdateService
{
    private readonly UserManager _userManager;

    public UserCreateUpdateService(UserManager userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<UserDto>> CreateAsync(UserCreateDto model, CancellationToken cancellationToken = default)
    {
        var result = await _userManager.CreateAsync(model.Name, model.Surname, model.UserName, model.ChatId, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error;
        }

        return result.Value.ToDto();
    }

    public async Task<Result<UserDto>> UpdateAsync(Guid id, UserUpdateDto model, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(User));
        }

        if (!model.Name.IsNullOrEmpty())
        {
            user.Name = model.Name;
        }

        if (!model.Surname.IsNullOrEmpty())
        {
            user.Surname = model.Surname;
        }

        if (!model.UserName.IsNullOrEmpty())
        {
            user.UserName = model.UserName;
        }

        if (!model.Culture.IsNullOrEmpty())
        {
            user.Culture = model.Culture;
        }

        if (model.NotificationsEnabled.HasValue)
        {
            user.NotificationsEnabled = model.NotificationsEnabled.Value;
        }

        return (await _userManager.UpdateAsync(user, cancellationToken)).ToDto();
    }
}
