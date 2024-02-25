using Eclipse.Common.Specifications.LogicalOperators;

namespace Eclipse.Common.Specifications;

public static class SpecificationExtensions
{
    public static Specification<T> And<T>(this Specification<T> left, Specification<T> right)
    {
        return new AndSpecification<T>(left, right);
    }
}