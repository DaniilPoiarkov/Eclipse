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

    public async Task<Result<UserDto>> CreateAsync(UserCreateDto createDto, CancellationToken cancellationToken = default)
    {
        var result = await _userManager.CreateAsync(createDto.Name, createDto.Surname, createDto.UserName, createDto.ChatId, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error;
        }

        return result.Value.ToDto();
    }

    public async Task<Result<UserDto>> UpdateAsync(Guid id, UserUpdateDto update, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(User));
        }

        if (!update.Name.IsNullOrEmpty())
        {
            user.Name = update.Name;
        }

        if (!update.Surname.IsNullOrEmpty())
        {
            user.Surname = update.Surname;
        }

        if (!update.UserName.IsNullOrEmpty())
        {
            user.UserName = update.UserName;
        }

        if (!update.Culture.IsNullOrEmpty())
        {
            user.Culture = update.Culture;
        }

        if (update.NotificationsEnabled.HasValue)
        {
            user.NotificationsEnabled = update.NotificationsEnabled.Value;
        }

        return (await _userManager.UpdateAsync(user, cancellationToken)).ToDto();
    }
}
