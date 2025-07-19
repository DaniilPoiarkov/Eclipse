using Eclipse.Application.Reports;
using Eclipse.Domain.Shared.MoodRecords;

using FluentAssertions;

using Xunit;

namespace Eclipse.Application.Tests.Reports;

public sealed class MoodStateExtensionsTests
{
    [Theory]
    [InlineData(MoodState.Good, 5)]
    [InlineData(MoodState.SlightlyGood, 4)]
    [InlineData(MoodState.Neutral, 3)]
    [InlineData(MoodState.SlightlyBad, 2)]
    [InlineData(MoodState.Bad, 1)]
    [InlineData((MoodState)10, 0)]
    public void ToScore_WhenConverted_ThenProperScoreReturned(MoodState state, int expected)
    {
        state.ToScore().Should().Be(expected);
    }
}
