using Eclipse.Common.Specifications;

using System.Linq.Expressions;

namespace Eclipse.Common.Tests.Specifications.Utils;

internal sealed class YLowerThenSpecification : Specification<TestObject>
{
    private readonly int _num;

    public YLowerThenSpecification(int num)
    {
        _num = num;
    }

    public override Expression<Func<TestObject, bool>> IsSatisfied()
    {
        return x => x.Y < _num;
    }
}
