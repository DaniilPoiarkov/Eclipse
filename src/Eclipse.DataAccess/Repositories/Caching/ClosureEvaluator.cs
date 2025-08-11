using System.Linq.Expressions;
using System.Reflection;

namespace Eclipse.DataAccess.Repositories.Caching;

internal sealed class ClosureEvaluator : ExpressionVisitor
{
    protected override Expression VisitMember(MemberExpression node)
    {
        if (node.Expression is not ConstantExpression constantExpression)
        {
            return base.VisitMember(node);
        }

        var field = constantExpression.Value;

        if (node.Member is FieldInfo fieldInfo)
        {
            return Expression.Constant(fieldInfo.GetValue(field), node.Type);
        }
        if (node.Member is PropertyInfo propertyInfo)
        {
            return Expression.Constant(propertyInfo.GetValue(field), node.Type);
        }

        return base.VisitMember(node);
    }
}
