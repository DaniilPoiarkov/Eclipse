using Eclipse.Common.Specifications.Visitors;

using System.Linq.Expressions;

namespace Eclipse.Common.Specifications.LogicalOperators;

internal abstract class LogicalSpecification<T> : Specification<T>
{
    protected readonly Specification<T> Left;

    protected readonly Specification<T> Right;

    protected LogicalSpecification(Specification<T> left, Specification<T> right)
    {
        Left = left;
        Right = right;
    }

    public override Expression<Func<T, bool>> IsSatisfied()
    {
        var param = Expression.Parameter(typeof(T), "x");

        var leftExpression = Left.IsSatisfied();
        var rightExpression = Right.IsSatisfied();

        var left = new ReplaceExpressionVisitor(leftExpression.Parameters[0], param)
            .Visit(leftExpression.Body);

        var right = new ReplaceExpressionVisitor(rightExpression.Parameters[0], param)
            .Visit(rightExpression.Body);
        
        return Expression.Lambda<Func<T, bool>>(
            Combine(left, right), param
        );
    }

    protected abstract BinaryExpression Combine(Expression left, Expression right);
}
