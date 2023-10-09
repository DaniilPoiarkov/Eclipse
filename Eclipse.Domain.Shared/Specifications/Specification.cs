using System.Linq.Expressions;

namespace Eclipse.Domain.Shared.Specifications;

public abstract class Specification<T>
{
    public abstract Expression<Func<T, bool>> IsSatisfied();

    public static implicit operator Expression<Func<T, bool>>(Specification<T> specification) => specification.IsSatisfied();

    public static implicit operator Func<T, bool>(Specification<T> specification) => specification.IsSatisfied().Compile();
}
