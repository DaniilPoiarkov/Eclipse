using System.Linq.Expressions;

namespace Eclipse.Domain.Shared.Specifications.LogicalOperators;

internal sealed class EmptySpecification<T> : Specification<T>
{
    public override Expression<Func<T, bool>> IsSatisfied()
    {
        return x => true;
    }
}
