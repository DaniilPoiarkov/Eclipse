namespace Eclipse.Domain.Shared.Specifications;

public static class SpecificationExtensions
{
    public static Specification<T> And<T>(this Specification<T> left, Specification<T> right)
    {
        return new AndSpecification<T>(left, right);
    }
}
