using System.Linq.Expressions;
using System.Reflection;

namespace Eclipse.DataAccess.Repositories.Caching;

internal sealed class ClosureEvaluator : ExpressionVisitor
{
    private static readonly int _maxDepth = 5;

    protected override Expression VisitMember(MemberExpression node)
    {
        var (succes, value) = TryExtractValue(node, 0);

        if (succes)
        {
            return Expression.Constant(value, node.Type);
        }

        return base.VisitMember(node);
    }

    private static (bool IsSuccess, object? Value) TryExtractValue(MemberExpression node, int depth)
    {
        if (depth >= _maxDepth)
        {
            return (false, null);
        }

        var (isSuccess, value) = TryExtractConstant(node.Expression, depth);

        if (!isSuccess)
        {
            return (false, null);
        }

        if (node.Member is FieldInfo fieldInfo)
        {
            return (true, fieldInfo.GetValue(value));
        }

        if (node.Member is PropertyInfo propertyInfo)
        {
            return (true, propertyInfo.GetValue(value));
        }

        return (false, null);
    }

    private static (bool IsSuccess, object? Value) TryExtractConstant(Expression? expression, int depth)
    {
        if (expression is ConstantExpression constantExpression)
        {
            return (true, constantExpression.Value);
        }

        if (expression is MemberExpression memberExpression)
        {
            return TryExtractValue(memberExpression, depth + 1);
        }

        return (false, null);
    }
}
