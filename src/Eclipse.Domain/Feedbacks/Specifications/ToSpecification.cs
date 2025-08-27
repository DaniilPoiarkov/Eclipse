using Eclipse.Common.Specifications;

using System.Linq.Expressions;

namespace Eclipse.Domain.Feedbacks.Specifications;

public sealed class ToSpecification : Specification<Feedback>
{
    private readonly DateTime _to;

    public ToSpecification(DateTime to)
    {
        _to = to;
    }

    public override Expression<Func<Feedback, bool>> IsSatisfied()
    {
        return f => f.CreatedAt <= _to;
    }
}
