using Eclipse.Domain.IdentityUsers;
using Eclipse.Tests.Generators;

using FluentAssertions;

using Xunit;

namespace Eclipse.Domain.Tests.IdentityUsers;

public class IdentityUserTests
{
    private readonly IdentityUser _sut;

    public IdentityUserTests()
    {
        _sut = IdentityUserGenerator.Generate(1).First();
    }

    [Fact]
    public void SetGmt_WhenGmtPlus3_ThenGmtPlus3Set()
    {
        var utc = DateTime.UtcNow;

        var hour = utc.Hour + 3 > 23
            ? utc.Hour - 21
            : utc.Hour + 3;

        var currentUserTime = new TimeOnly(hour, utc.Minute);
        var expected = new TimeSpan(3, 0, 0);

        _sut.SetGmt(currentUserTime);

        _sut.Gmt.Should().Be(expected);
    }

    [Fact]
    public void SetGmt_WhenGmtMinus4_ThenGmtMinus4Set()
    {
        var utc = DateTime.UtcNow;

        var hour = utc.Hour - 4 < 0
            ? utc.Hour + 20
            : utc.Hour - 4;

        var currentUserTime = new TimeOnly(hour, utc.Minute);

        var expected = new TimeSpan(-4, 0, 0);

        _sut.SetGmt(currentUserTime);

        _sut.Gmt.Should().Be(expected);
    }
}
