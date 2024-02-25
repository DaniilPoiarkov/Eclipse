using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Eclipse.Domain.Shared.Specifications.Visitors;

internal sealed class ReplaceExpressionVisitor : ExpressionVisitor
{
    private readonly Expression _old;

    private readonly Expression _new;

    public ReplaceExpressionVisitor(Expression old, Expression @new)
    {
        _old = old;
        _new = @new;
    }

    [return: NotNullIfNotNull(nameof(node))]
    public override Expression? Visit(Expression? node)
    {
        if (node == _old)
        {
            return _new;
        }

        return base.Visit(node);
    }
}
