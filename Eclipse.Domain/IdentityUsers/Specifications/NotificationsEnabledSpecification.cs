using Eclipse.Domain.Shared.Specifications;

using System.Linq.Expressions;

namespace Eclipse.Domain.IdentityUsers.Specifications;

public sealed class NotificationsEnabledSpecification : Specification<IdentityUser>
{
    private readonly bool _notificationsEnabled;

    public NotificationsEnabledSpecification(bool notificationsEnabled)
    {
        _notificationsEnabled = notificationsEnabled;
    }

    public override Expression<Func<IdentityUser, bool>> IsSatisfied()
    {
        return _notificationsEnabled
            ? u => u.NotificationsEnabled
            : u => true;
    }
}
