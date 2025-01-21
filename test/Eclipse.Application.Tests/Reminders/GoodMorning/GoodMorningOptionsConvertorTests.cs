using Eclipse.Application.Reminders.GoodMorning;
using Eclipse.Tests.Generators;

using FluentAssertions;

using Xunit;

namespace Eclipse.Application.Tests.Reminders.GoodMorning;

public sealed class GoodMorningOptionsConvertorTests
{
    private readonly GoodMorningOptionsConvertor _sut;

    public GoodMorningOptionsConvertorTests()
    {
        _sut = new GoodMorningOptionsConvertor();
    }

    [Fact]
    public void Convert_WhenCalled_ThenConverted()
    {
        var user = UserGenerator.Get();

        var result = _sut.Convert(user);

        result.UserId.Should().Be(user.Id);
        result.Gmt.Should().Be(user.Gmt);
    }
}
