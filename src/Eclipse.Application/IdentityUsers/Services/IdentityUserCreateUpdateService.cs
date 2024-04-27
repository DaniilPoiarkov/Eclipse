using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Common.Results;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Domain.Shared.Errors;

namespace Eclipse.Application.IdentityUsers.Services;

internal sealed class IdentityUserCreateUpdateService : IIdentityUserCreateUpdateService
{
    private readonly IdentityUserManager _userManager;

    public IdentityUserCreateUpdateService(IdentityUserManager userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<IdentityUserDto>> CreateAsync(IdentityUserCreateDto createDto, CancellationToken cancellationToken = default)
    {
        var result = await _userManager.CreateAsync(createDto.Name, createDto.Surname, createDto.UserName, createDto.ChatId, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error;
        }

        return result.Value.ToDto();
    }

    public async Task<Result<IdentityUserDto>> UpdateAsync(Guid id, IdentityUserUpdateDto update, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(IdentityUser));
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
