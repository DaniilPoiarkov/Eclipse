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
        var result = await _userManager.CreateAsync(createDto.Name, createDto.Surname, createDto.Username, createDto.ChatId, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error;
        }

        return result.Value.ToDto();
    }

    public async Task<Result<IdentityUserDto>> UpdateAsync(Guid id, IdentityUserUpdateDto updateDto, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(IdentityUser));
        }

        user.Name = updateDto.Name is null
            ? user.Name
            : updateDto.Name;

        user.Surname = updateDto.Surname is null
            ? user.Surname
            : updateDto.Surname;

        user.Username = updateDto.Username is null
            ? user.Username
            : updateDto.Username;

        if (!string.IsNullOrEmpty(updateDto.Culture))
        {
            user.Culture = updateDto.Culture;
        }

        if (updateDto.NotificationsEnabled.HasValue)
        {
            user.NotificationsEnabled = updateDto.NotificationsEnabled.Value;
        }

        return (await _userManager.UpdateAsync(user, cancellationToken)).ToDto();
    }
}
