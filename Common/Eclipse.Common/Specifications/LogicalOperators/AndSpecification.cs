using System.Linq.Expressions;

namespace Eclipse.Common.Specifications.LogicalOperators;

internal sealed class AndSpecification<T> : LogicalSpecification<T>
{
    public AndSpecification(Specification<T> left, Specification<T> right)
        : base(left, right) { }

    protected override BinaryExpression Combine(Expression left, Expression right)
    {
        return Expression.AndAlso(left, right);
    }
}
