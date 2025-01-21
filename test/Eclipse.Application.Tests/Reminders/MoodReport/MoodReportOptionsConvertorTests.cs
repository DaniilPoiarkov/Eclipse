using Eclipse.Application.Reminders.MoodReport;
using Eclipse.Tests.Generators;

using FluentAssertions;

using Xunit;

namespace Eclipse.Application.Tests.Reminders.MoodReport;

public sealed class MoodReportOptionsConvertorTests
{
    private readonly MoodReportOptionsConvertor _sut;

    public MoodReportOptionsConvertorTests()
    {
        _sut = new MoodReportOptionsConvertor();
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
