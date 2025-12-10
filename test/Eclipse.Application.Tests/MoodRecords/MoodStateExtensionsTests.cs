using Eclipse.Application.MoodRecords;
using Eclipse.Domain.Shared.MoodRecords;

using FluentAssertions;

using Xunit;

namespace Eclipse.Application.Tests.MoodRecords;

public sealed class MoodStateExtensionsTests
{
    [Theory]
    [InlineData(MoodState.Amazing, 10)]
    [InlineData(MoodState.Excellent, 9)]
    [InlineData(MoodState.VeryGood, 8)]
    [InlineData(MoodState.Good, 7)]
    [InlineData(MoodState.Fine, 6)]
    [InlineData(MoodState.Neutral, 5)]
    [InlineData(MoodState.Poor, 4)]
    [InlineData(MoodState.Bad, 3)]
    [InlineData(MoodState.VeryBad, 2)]
    [InlineData(MoodState.Terrible, 1)]
    [InlineData((MoodState)10, 0)]
    public void ToScore_WhenConverted_ThenProperScoreReturned(MoodState state, int expected)
    {
        state.ToScore().Should().Be(expected);
    }
}
