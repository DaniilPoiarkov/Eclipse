using System.Linq.Expressions;
using System.Reflection;

namespace Eclipse.DataAccess.Repositories.Caching;

internal sealed class ClosureEvaluator : ExpressionVisitor
{
    private static readonly int _maxDepth = 5;

    protected override Expression VisitMember(MemberExpression node)
    {
        if (!TryExtractValue(node, 0, out var value))
        {
            return base.VisitMember(node);
        }

        return Expression.Constant(value, node.Type);
    }

    private static bool TryExtractValue(MemberExpression node, int depth, out object? value)
    {
        value = null;

        if (depth >= _maxDepth)
        {
            return false;
        }

        if (node.Expression is ConstantExpression constantExpression)
        {
            value = constantExpression.Value;
        }

        if (node.Expression is MemberExpression memberExpression)
        {
            TryExtractValue(memberExpression, depth + 1, out value);
        }

        if (value is null)
        {
            return false;
        }

        if (node.Member is FieldInfo fieldInfo)
        {
            value = fieldInfo.GetValue(value);
            return true;
        }

        if (node.Member is PropertyInfo propertyInfo)
        {
            value = propertyInfo.GetValue(value);
            return true;
        }

        return false;
    }
}
