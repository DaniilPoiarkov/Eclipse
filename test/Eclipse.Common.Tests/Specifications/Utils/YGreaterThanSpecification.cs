using Eclipse.Common.Specifications;

using System.Linq.Expressions;

namespace Eclipse.Domain.Tests.Specifications.Utils;

internal sealed class YGreaterThanSpecification : Specification<TestObject>
{
    private readonly int _y;

    public YGreaterThanSpecification(int y)
    {
        _y = y;
    }

    public override Expression<Func<TestObject, bool>> IsSatisfied()
    {
        return testObject => testObject.Y > _y;
    }
}
