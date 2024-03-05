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
}
