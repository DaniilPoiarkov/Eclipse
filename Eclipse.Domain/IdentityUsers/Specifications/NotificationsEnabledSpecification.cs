using Eclipse.Domain.Shared.Specifications;

using System.Linq.Expressions;

namespace Eclipse.Domain.IdentityUsers.Specifications;

public sealed class NotificationsEnabledSpecification : Specification<IdentityUser>
{
    public override Expression<Func<IdentityUser, bool>> IsSatisfied()
    {
        return u => u.NotificationsEnabled;
    }
}
