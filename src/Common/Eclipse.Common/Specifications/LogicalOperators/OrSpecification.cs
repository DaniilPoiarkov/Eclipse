using System.Linq.Expressions;

namespace Eclipse.Common.Specifications.LogicalOperators;

internal sealed class OrSpecification<T> : LogicalSpecification<T>
{
    public OrSpecification(Specification<T> left, Specification<T> right)
        : base(left, right) { }

    protected override BinaryExpression Combine(Expression left, Expression right)
    {
        return Expression.OrElse(left, right);
    }
}
