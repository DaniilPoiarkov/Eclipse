using Eclipse.Application.Extensions;

using FluentAssertions;

using Xunit;

namespace Eclipse.Application.Tests.Extensions;

public class PrimitivesExtensionsTests
{
    [Fact]
    public void ParseAsTimeOnly_WhenValueCanBeParsed_ThenValidTimeOnlyResultReturned()
    {
        var str = "17:45";

        var expected = new TimeOnly(17, 45);

        var parsed = str.TryParseAsTimeOnly(out var time);

        parsed.Should().BeTrue();
        time.Should().Be(expected);
    }
}
