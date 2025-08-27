using Eclipse.Common.Specifications;

using System.Linq.Expressions;

namespace Eclipse.Domain.Feedbacks.Specifications;

public sealed class PostedByUsersSpecification : Specification<Feedback>
{
    private readonly IEnumerable<Guid> _userIds;

    public PostedByUsersSpecification(IEnumerable<Guid> userIds)
    {
        _userIds = userIds;
    }

    public override Expression<Func<Feedback, bool>> IsSatisfied()
    {
        return f => _userIds.Contains(f.UserId);
    }
}
