using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Specifications;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Users;

internal static class GetUsersRequestExtensions
{
    public static Specification<User> ToSpecification(this GetUsersRequest request)
    {
        return Specification<User>.Empty
            .AndIf(!request.Name.IsNullOrEmpty(), new CustomSpecification<User>(u => u.Name.Contains(request.Name ?? string.Empty)))
            .AndIf(!request.UserName.IsNullOrEmpty(), new CustomSpecification<User>(u => u.UserName.Contains(request.UserName ?? string.Empty)))
            .AndIf(request.NotificationsEnabled, new CustomSpecification<User>(u => u.NotificationsEnabled))
            .AndIf(request.OnlyActive, new CustomSpecification<User>(u => u.IsEnabled))
            .AndIf(!request.UserIds.IsNullOrEmpty(), new CustomSpecification<User>(u => request.UserIds.Contains(u.Id)));
    }
}
