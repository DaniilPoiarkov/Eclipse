using Eclipse.Common.Specifications;
using Eclipse.Domain.Tests.Specifications.Utils;

using FluentAssertions;

using Xunit;

namespace Eclipse.Domain.Tests.Specifications;

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
        var greaterThanValue = 5;
        var expectedCount = 5;

        var specification = new XGreaterThanSpecification(greaterThanValue)
            .And(new YGreaterThanSpecification(greaterThanValue));

        var results = _objects.Where(specification).ToArray();

        results.Length.Should().Be(expectedCount);
        results.All(x => x.X > greaterThanValue).Should().BeTrue();
        results.All(x => x.Y > greaterThanValue).Should().BeTrue();
    }
}
