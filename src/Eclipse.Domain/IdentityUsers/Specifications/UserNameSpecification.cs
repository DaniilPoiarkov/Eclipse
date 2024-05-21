using Eclipse.Common.Specifications;

using System.Linq.Expressions;

namespace Eclipse.Domain.IdentityUsers.Specifications;

public sealed class UserNameSpecification : Specification<IdentityUser>
{
    private readonly string _userName;

    public UserNameSpecification(string userName)
    {
        _userName = userName;
    }

    public override Expression<Func<IdentityUser, bool>> IsSatisfied()
    {
        return u => u.UserName.Contains(_userName);
    }
}
