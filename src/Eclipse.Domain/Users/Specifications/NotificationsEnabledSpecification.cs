using Eclipse.Common.Specifications;

using System.Linq.Expressions;

namespace Eclipse.Domain.Users.Specifications;

public sealed class NotificationsEnabledSpecification : Specification<User>
{
    public override Expression<Func<User, bool>> IsSatisfied()
    {
        return u => u.NotificationsEnabled;
    }
}
