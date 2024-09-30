using Eclipse.Common.Tests.Extensions.TestData;

using FluentAssertions;

namespace Eclipse.Common.Tests.Extensions;

public sealed class ObjectExtensionsTests
{
    [Theory]
    [InlineData("e0f36e9c-c7f0-442d-a19d-ea1103e72dc7", "e0f36e9c-c7f0-442d-a19d-ea1103e72dc7")]
    [InlineData("00000000-0000-0000-0000-000000000000", "00000000-0000-0000-0000-000000000000")]
    [InlineData("invalid guid", "00000000-0000-0000-0000-000000000000")]
    [InlineData(null, "00000000-0000-0000-0000-000000000000")]
    public void ToGuid_WhenCalled_ThenProperResultReturned(object? value, string expected)
    {
        value.ToGuid().ToString().Should().Be(expected);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(-123931, -123931)]
    [InlineData(0, 0)]
    [InlineData("something invalid", 0)]
    [InlineData(null, 0)]
    public void ToLong_WhenCalled_ThenProperResultReturned(object? value, long expected)
    {
        value.ToLong().Should().Be(expected);
    }

    [Theory]
    [ClassData(typeof(ToDateTimeTestData))]
    public void ToDateTime_WhenCalled_ThenProperResultReturned(object? value, DateTime expected)
    {
        value.ToDateTime().Should().Be(expected);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, true)]
    [InlineData(null, false)]
    [InlineData("true", true)]
    public void ToBool_WhenCalled_ThenProperResultReturned(object? value, bool expected)
    {
        value.ToBool().Should().Be(expected);
    }
}
