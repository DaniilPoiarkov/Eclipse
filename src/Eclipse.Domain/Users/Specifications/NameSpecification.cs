using Eclipse.Common.Specifications;

using System.Linq.Expressions;

namespace Eclipse.Domain.Users.Specifications;

public sealed class NameSpecification : Specification<User>
{
    private readonly string _name;

    public NameSpecification(string name)
    {
        _name = name;
    }

    public override Expression<Func<User, bool>> IsSatisfied()
    {
        return u => u.Name.Contains(_name);
    }
}
