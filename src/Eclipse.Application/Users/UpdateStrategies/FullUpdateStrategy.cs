using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Results;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Users.UpdateStrategies;

internal sealed class FullUpdateStrategy : IUserUpdateStrategy
{
    private readonly UserUpdateDto _model;

    private readonly UserManager _userManager;

    public FullUpdateStrategy(UserUpdateDto model, UserManager userManager)
    {
        _model = model;
        _userManager = userManager;
    }

    public async Task<Result<User>> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        if (_model.Name.IsNullOrEmpty())
        {
            return Error.Validation("Users.Update", "{0}IsRequired", nameof(_model.Name));
        }

        if (_model.UserName.IsNullOrEmpty())
        {
            return Error.Validation("Users.Update", "{0}IsRequired", nameof(_model.UserName));
        }

        var result = await UpdateUserNameAsync(user, cancellationToken);

        if (!result.IsSuccess)
        {
            return result;
        }

        user.Name = _model.Name;
        user.NotificationsEnabled = _model.NotificationsEnabled;
        user.Surname = _model.Surname ?? string.Empty;
        user.Culture = _model.Culture ?? string.Empty;

        return user;
    }

    private async Task<Result<User>> UpdateUserNameAsync(User user, CancellationToken cancellationToken)
    {
        if (user.UserName.Equals(_model.UserName))
        {
            return user;
        }

        var userNameReserved = await _userManager.FindByUserNameAsync(_model.UserName, cancellationToken);

        if (userNameReserved is not null && userNameReserved.ChatId != user.ChatId)
        {
            return Error.Validation("Users.Update", "User:DuplicateData", nameof(_model.UserName));
        }

        user.UserName = _model.UserName;

        return user;
    }
}
