using Eclipse.Domain.Shared.Specifications;

using System.Linq.Expressions;

namespace Eclipse.Domain.IdentityUsers.Specifications;

public sealed class NameSpecification : Specification<IdentityUser>
{
    private readonly string _name;

    public NameSpecification(string name)
    {
        _name = name;
    }

    public override Expression<Func<IdentityUser, bool>> IsSatisfied()
    {
        return u => u.Name.Contains(_name, StringComparison.OrdinalIgnoreCase);
    }
}
