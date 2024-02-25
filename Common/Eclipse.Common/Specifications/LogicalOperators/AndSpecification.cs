using Eclipse.Common.Specifications.Visitors;

using System.Linq.Expressions;

namespace Eclipse.Common.Specifications.LogicalOperators;

internal sealed class AndSpecification<T> : Specification<T>
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
        var param = Expression.Parameter(typeof(T), "x");

        var leftExpression = _left.IsSatisfied();
        var rightExpression = _right.IsSatisfied();

        var left = new ReplaceExpressionVisitor(leftExpression.Parameters[0], param)
            .Visit(leftExpression.Body);

        var right = new ReplaceExpressionVisitor(rightExpression.Parameters[0], param)
            .Visit(rightExpression.Body);

        return Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(left, right), param
        );
    }
}
