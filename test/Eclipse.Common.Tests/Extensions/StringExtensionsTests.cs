using FluentAssertions;

namespace Eclipse.Common.Tests.Extensions;

public sealed class StringExtensionsTests
{
    [Fact]
    public void TryParseAsTimeOnly_WhenValueCanBeParsed_ThenValidTimeOnlyResultReturned()
    {
        var str = "17:45";

        var expected = new TimeOnly(17, 45);

        var parsed = str.TryParseAsTimeOnly(out var time);

        parsed.Should().BeTrue();
        time.Should().Be(expected);
    }

    [Fact]
    public void TryParseAsTimeOnly_WhenCannotBeParsed_ThenDefaultValueReturned()
    {
        var str = "17:hh";

        var parsed = str.TryParseAsTimeOnly(out var time);

        parsed.Should().BeFalse();
        time.Should().Be(default);
    }

    [Fact]
    public void TryParseAsTimeOnly_WhenHoursAndMinutesOutOfRange_ThenDefaultValueReturned()
    {
        var str = "222:333";

        var parsed = str.TryParseAsTimeOnly(out var time);

        parsed.Should().BeFalse();
        time.Should().Be(default);
    }
}