using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Results;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Users.UpdateStrategies;

internal sealed class PartialUpdateStrategy : IUserUpdateStrategy
{
    private readonly UserPartialUpdateDto _model;

    private readonly UserManager _userManager;

    public PartialUpdateStrategy(UserPartialUpdateDto model, UserManager userManager)
    {
        _model = model;
        _userManager = userManager;
    }

    public async Task<Result<User>> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        var result = UpdateName(user);

        if (!result.IsSuccess)
        {
            return result;
        }

        if (_model.SurnameChanged)
        {
            user.Surname = _model.Surname ?? string.Empty;
        }

        result = await UpdateUserNameAsync(user, cancellationToken);

        if (!result.IsSuccess)
        {
            return result;
        }

        if (_model.CultureChanged)
        {
            user.Culture = _model.Culture ?? string.Empty;
        }

        if (_model.NotificationsEnabledChanged)
        {
            user.NotificationsEnabled = _model.NotificationsEnabled;
        }

        if (_model.GmtChanged)
        {
            user.SetGmt(_model.Gmt);
        }

        return user;
    }

    private Result<User> UpdateName(User user)
    {
        if (!_model.NameChanged)
        {
            return user;
        }

        if (_model.Name.IsNullOrEmpty())
        {
            return Error.Validation("Users.Update", "{0}IsRequired", nameof(_model.Name));
        }

        user.Name = _model.Name;

        return user;
    }

    private async Task<Result<User>> UpdateUserNameAsync(User user, CancellationToken cancellationToken)
    {
        if (!_model.UserNameChanged || user.UserName.Equals(_model.UserName))
        {
            return user;
        }

        if (_model.UserName.IsNullOrEmpty())
        {
            return Error.Validation("Users.Update", "{0}IsRequired", nameof(_model.UserName));
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
