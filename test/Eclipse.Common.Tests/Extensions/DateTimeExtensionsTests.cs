using FluentAssertions;

namespace Eclipse.Common.Tests.Extensions;

public sealed class DateTimeExtensionsTests
{
    [Theory]
    [InlineData(2024, 9, 28, DayOfWeek.Sunday, 2024, 9, 29)]
    [InlineData(2024, 9, 28, DayOfWeek.Monday, 2024, 9, 30)]
    [InlineData(2024, 9, 28, DayOfWeek.Tuesday, 2024, 10, 1)]
    [InlineData(2024, 12, 30, DayOfWeek.Wednesday, 2025, 1, 1)]
    public void NextDayOfWeek_WhenCalled_ThenProperDateTimeReturned(int year, int month, int day, DayOfWeek dayOfWeek, int expectedYear, int expectedMonth, int expectedDay)
    {
        var dateTime = new DateTime(year, month, day);
        var expected = new DateTime(expectedYear, expectedMonth, expectedDay);

        var actual = dateTime.NextDayOfWeek(dayOfWeek);

        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(23, 59)]
    public void GetTime_WhenCalled_ThenProperTimeOnlyReturned(int hour, int minutes)
    {
        var utcNow = DateTime.UtcNow;
        var expected = new TimeOnly(hour, minutes);

        var dateTime = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, hour, minutes, 0);

        var actual = dateTime.GetTime();

        actual.Should().Be(expected);
    }
}
