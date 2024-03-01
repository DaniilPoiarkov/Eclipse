using Eclipse.Common.Specifications;
using Eclipse.Common.Tests.Specifications.Utils;

using FluentAssertions;

namespace Eclipse.Common.Tests.Specifications;

public class AndNotSpecificationTests
{
    private readonly IEnumerable<TestObject> _objects;

    public AndNotSpecificationTests()
    {
        _objects = Enumerable.Range(1, 10)
            .Select(x => new TestObject
            {
                X = x,
                Y = x,
            });
    }

    [Fact]
    public void AndNot_WhenApplied_ThenBathSpecificationsAppliedProperly()
    {
        var xGreaterThanValue = 5;
        var yGreaterThanValue = 7;
        var expectedCount = 2;

        var specification = new XGreaterThanSpecification(xGreaterThanValue)
            .AndNot(new YGreaterThanSpecification(yGreaterThanValue));

        var results = _objects.Where(specification).ToArray();

        results.Length.Should().Be(expectedCount);
        results.All(x => x.X > xGreaterThanValue).Should().BeTrue();
        results.All(x => x.Y <= yGreaterThanValue).Should().BeTrue();
    }
}
