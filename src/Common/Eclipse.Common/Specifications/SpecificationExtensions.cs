using Eclipse.Common.Specifications.LogicalOperators;

namespace Eclipse.Common.Specifications;

public static class SpecificationExtensions
{
    /// <summary>
    /// Specifiies that element should match both specifications
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Specification<T> And<T>(this Specification<T> left, Specification<T> right)
    {
        return new AndSpecification<T>(left, right);
    }

    /// <summary>
    /// Specifiies that element should match either left or right specification
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Specification<T> Or<T>(this Specification<T> left, Specification<T> right)
    {
        return new OrSpecification<T>(left, right);
    }

    /// <summary>
    /// Ors if.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="left">The left.</param>
    /// <param name="condition">The condition.</param>
    /// <param name="condition">if set to <c>true</c> [condition].</param>
    /// <param name="right">The right.</param>
    /// <returns></returns>
    public static Specification<T> OrIf<T>(this Specification<T> left, bool condition, Specification<T> right)
    {
        if (condition)
        {
            return left.Or(right);
        }

        return left;
    }

    /// <summary>
    /// Specifies that element should match left specification and not right specification
    ///   <br />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>
    ///   <br />
    /// </returns>
    public static Specification<T> AndNot<T>(this Specification<T> left, Specification<T> right)
    {
        return new AndNotSpecification<T>(left, right);
    }

    public static Specification<T> AndIf<T>(this Specification<T> left, bool condition, Specification<T> right)
    {
        if (condition)
        {
            return left.And(right);
        }

        return left;
    }
}
