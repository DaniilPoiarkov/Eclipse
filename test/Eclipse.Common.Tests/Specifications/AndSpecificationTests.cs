using Eclipse.Common.Specifications;
using Eclipse.Common.Tests.Specifications.Utils;

using FluentAssertions;

namespace Eclipse.Common.Tests.Specifications;

public sealed class AndSpecificationTests
{
    private readonly IEnumerable<TestObject> _objects;

    public AndSpecificationTests()
    {
        _objects = Enumerable.Range(1, 10)
            .Select(x => new TestObject
            {
                X = x,
                Y = x,
            });
    }

    [Fact]
    public void AndSpecification_WhenApplied_ThenBothSpecificationMustMatch()
    {
        var xGreaterThanValue = 5;
        var yGreaterThanValue = 7;
        var expectedCount = 3;

        var specification = new XGreaterThanSpecification(xGreaterThanValue)
            .And(new YGreaterThanSpecification(yGreaterThanValue));

        var results = _objects.Where(specification).ToArray();

        results.Length.Should().Be(expectedCount);
        results.All(x => x.X > xGreaterThanValue).Should().BeTrue();
        results.All(x => x.Y > yGreaterThanValue).Should().BeTrue();
    }
}
