using Eclipse.Domain.Shared.Specifications;

using System;
using System.Linq.Expressions;

namespace Eclipse.Domain.IdentityUsers.Specifications;

public sealed class NameSpecification : Specification<IdentityUser>
{
    private readonly string? _name;

    public NameSpecification(string? name)
    {
        _name = name;
    }

    public override Expression<Func<IdentityUser, bool>> IsSatisfied()
    {
        return _name.IsNullOrEmpty()
            ? u => true
            : u => u.Name.ContainsOrdinalIgnoreCase(_name);
    }
}
