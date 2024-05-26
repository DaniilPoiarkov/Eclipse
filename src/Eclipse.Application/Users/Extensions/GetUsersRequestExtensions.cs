using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Specifications;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Specifications;

namespace Eclipse.Application.Users.Extensions;

internal static class GetUsersRequestExtensions
{
    public static Specification<User> GetSpecification(this GetUsersRequest request)
    {
        var specification = Specification<User>.Empty;

        if (request is null)
        {
            return specification;
        }

        if (!request.Name.IsNullOrEmpty())
        {
            specification = specification
                .And(new NameSpecification(request.Name));
        }

        if (!request.UserName.IsNullOrEmpty())
        {
            specification = specification
                .And(new UserNameSpecification(request.UserName));
        }

        if (request.NotificationsEnabled)
        {
            specification = specification
                .And(new NotificationsEnabledSpecification());
        }

        return specification;
    }
}
