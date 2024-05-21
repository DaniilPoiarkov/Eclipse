using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Common.Specifications;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Domain.IdentityUsers.Specifications;

namespace Eclipse.Application.IdentityUsers.Extensions;

internal static class GetUsersRequestExtensions
{
    public static Specification<IdentityUser> GetSpecification(this GetUsersRequest request)
    {
        var specification = Specification<IdentityUser>.Empty;

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
