using Eclipse.Common.Specifications;

using System.Linq.Expressions;

namespace Eclipse.Domain.Feedbacks.Specifications;

public sealed class FromSpecification : Specification<Feedback>
{
    private readonly DateTime _from;

    public FromSpecification(DateTime from)
    {
        _from = from;
    }

    public override Expression<Func<Feedback, bool>> IsSatisfied()
    {
        return f => f.CreatedAt >= _from;
    }
}
