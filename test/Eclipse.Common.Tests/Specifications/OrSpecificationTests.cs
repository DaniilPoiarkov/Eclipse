using Eclipse.Common.Specifications;
using Eclipse.Common.Tests.Specifications.Utils;

using FluentAssertions;

namespace Eclipse.Common.Tests.Specifications;

public class OrSpecificationTests
{
    private readonly IEnumerable<TestObject> _objects;

    public OrSpecificationTests()
    {
        _objects = Enumerable.Range(1, 10)
            .Select(x => new TestObject
            {
                X = x,
                Y = x,
            });
    }

    [Fact]
    public void OrSpecification_WhenApplied_ThenEitherLeft_OrRightSpecificationMustMatch()
    {
        var xGreaterThanValue = 5;
        var yLowerThanValue = 2;
        var expectedCount = 6;

        var specification = new XGreaterThanSpecification(xGreaterThanValue)
            .Or(new YLowerThenSpecification(yLowerThanValue));

        var results = _objects.Where(specification).ToArray();

        results.Length.Should().Be(expectedCount, "Expecting to have 1 object with values (1, 1) and 5 more starting from (6, 6) up to (10, 10) inclusively.");
        results.All(x => x.X > xGreaterThanValue || x.Y < yLowerThanValue).Should().BeTrue();
    }
}
