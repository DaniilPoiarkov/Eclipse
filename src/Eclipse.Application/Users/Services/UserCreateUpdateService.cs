using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Users.UpdateStrategies;
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

    public Task<Result<UserDto>> UpdateAsync(Guid id, UserUpdateDto model, CancellationToken cancellationToken = default)
    {
        return UpdateInternal(id, new FullUpdateStrategy(model, _userManager), cancellationToken);
    }

    public Task<Result<UserDto>> UpdatePartialAsync(Guid id, UserPartialUpdateDto model, CancellationToken cancellationToken = default)
    {
        return UpdateInternal(id, new PartialUpdateStrategy(model, _userManager), cancellationToken);
    }

    private async Task<Result<UserDto>> UpdateInternal(Guid id, IUserUpdateStrategy strategy, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(id, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound(typeof(User));
        }

        var result = await strategy.UpdateAsync(user, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error;
        }

        return (await _userManager.UpdateAsync(user, cancellationToken)).ToDto();
    }
}
