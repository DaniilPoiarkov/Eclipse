using System.Linq.Expressions;

namespace Eclipse.Common.Specifications;

/// <summary>
/// Just a wrapper for custom expressions
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="Specification&lt;T&gt;" />
public sealed class CustomSpecification<T> : Specification<T>
{
    private readonly Expression<Func<T, bool>> _expression;

    public CustomSpecification(Expression<Func<T, bool>> expression)
    {
        _expression = expression;
    }

    public override Expression<Func<T, bool>> IsSatisfied()
    {
        return _expression;
    }
}
