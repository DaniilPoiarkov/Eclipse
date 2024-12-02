using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Users.UpdateStrategies;
using Eclipse.Common.Results;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Users.Services;

internal sealed class UserCreateUpdateService : IUserCreateUpdateService
{
    private readonly UserManager _userManager;

    private readonly IUserRepository _userRepository;

    public UserCreateUpdateService(UserManager userManager, IUserRepository userRepository)
    {
        _userManager = userManager;
        _userRepository = userRepository;
    }

    public async Task<Result<UserDto>> CreateAsync(UserCreateDto model, CancellationToken cancellationToken = default)
    {
        return await _userManager.CreateAsync(model.Name, model.Surname, model.UserName, model.ChatId, cancellationToken)
            .BindAsync(user => user.ToDto());
    }

    public Task<Result<UserDto>> UpdateAsync(Guid id, UserUpdateDto model, CancellationToken cancellationToken = default)
    {
        return UpdateInternal(id, new FullUpdateStrategy(model, _userRepository), cancellationToken);
    }

    public Task<Result<UserDto>> UpdatePartialAsync(Guid id, UserPartialUpdateDto model, CancellationToken cancellationToken = default)
    {
        return UpdateInternal(id, new PartialUpdateStrategy(model, _userRepository), cancellationToken);
    }

    private async Task<Result<UserDto>> UpdateInternal(Guid id, IUserUpdateStrategy strategy, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindAsync(id, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound<User>();
        }

        return await strategy.UpdateAsync(user, cancellationToken)
            .TapAsync(user => _userRepository.UpdateAsync(user, cancellationToken))
            .BindAsync(user => user.ToDto());
    }
}
