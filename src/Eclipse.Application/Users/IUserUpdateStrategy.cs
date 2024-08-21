using Eclipse.Common.Results;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Users;

internal interface IUserUpdateStrategy
{
    Task<Result<User>> UpdateAsync(User user, CancellationToken cancellationToken = default);
}
