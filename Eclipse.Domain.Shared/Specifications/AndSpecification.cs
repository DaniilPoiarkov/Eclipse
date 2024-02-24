using System.Linq.Expressions;

namespace Eclipse.Domain.Shared.Specifications;

internal class AndSpecification<T> : Specification<T>
{
    private readonly Specification<T> _left;

    private readonly Specification<T> _right;

    public AndSpecification(Specification<T> left, Specification<T> right)
    {
        _left = left;
        _right = right;
    }

    public override Expression<Func<T, bool>> IsSatisfied()
    {
        return Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(
                Expression.Invoke(_left),
                Expression.Invoke(_right)
            )
        );
    }
}
