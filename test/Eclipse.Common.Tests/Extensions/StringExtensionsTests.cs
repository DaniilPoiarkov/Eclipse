using FluentAssertions;

namespace Eclipse.Common.Tests.Extensions;

public sealed class StringExtensionsTests
{
    [Theory]
    [InlineData("17:45", 17, 45)]
    [InlineData("00:01", 0, 1)]
    [InlineData("12:00", 12, 0)]
    [InlineData("23:59", 23, 59)]
    public void TryParseAsTimeOnly_WhenValueCanBeParsed_ThenValidTimeOnlyResultReturned(string str, int hours, int minutes)
    {
        var expected = new TimeOnly(hours, minutes);

        var parsed = str.TryParseAsTimeOnly(out var time);

        parsed.Should().BeTrue();
        time.Should().Be(expected);
    }

    [Theory]
    [InlineData("17:hh")]
    [InlineData("222:333")]
    [InlineData("00:qwe")]
    [InlineData("00.01")]
    public void TryParseAsTimeOnly_WhenFormatInvalid_ThenDefaultValueReturned(string str)
    {
        var parsed = str.TryParseAsTimeOnly(out var time);

        parsed.Should().BeFalse();
        time.Should().Be(default);
    }
}
