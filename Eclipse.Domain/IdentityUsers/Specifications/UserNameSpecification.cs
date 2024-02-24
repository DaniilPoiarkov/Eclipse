using Eclipse.Domain.Shared.Specifications;

using System.Linq.Expressions;

namespace Eclipse.Domain.IdentityUsers.Specifications;

public sealed class UserNameSpecification : Specification<IdentityUser>
{
    private readonly string? _userName;

    public UserNameSpecification(string? userName)
    {
        _userName = userName;
    }

    public override Expression<Func<IdentityUser, bool>> IsSatisfied()
    {
        return _userName.IsNullOrEmpty()
            ? u => true
            : u => u.Username.ContainsOrdinalIgnoreCase(_userName);
    }
}
