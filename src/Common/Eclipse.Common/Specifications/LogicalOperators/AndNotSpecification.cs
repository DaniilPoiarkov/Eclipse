using System.Linq.Expressions;

namespace Eclipse.Common.Specifications.LogicalOperators;

internal sealed class AndNotSpecification<T> : LogicalSpecification<T>
{
    public AndNotSpecification(Specification<T> left, Specification<T> right)
        : base(left, right) { }

    protected override BinaryExpression Combine(Expression left, Expression right)
    {
        var not = Expression.Not(right);
        return Expression.AndAlso(left, not);
    }
}
