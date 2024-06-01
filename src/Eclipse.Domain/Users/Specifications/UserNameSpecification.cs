using Eclipse.Common.Specifications;

using System.Linq.Expressions;

namespace Eclipse.Domain.Users.Specifications;

public sealed class UserNameSpecification : Specification<User>
{
    private readonly string _userName;

    public UserNameSpecification(string userName)
    {
        _userName = userName;
    }

    public override Expression<Func<User, bool>> IsSatisfied()
    {
        return u => u.UserName.Contains(_userName);
    }
}
