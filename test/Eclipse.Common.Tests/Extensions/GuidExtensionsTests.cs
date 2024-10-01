using FluentAssertions;

namespace Eclipse.Common.Tests.Extensions;

public sealed class GuidExtensionsTests
{
    [Theory]
    [InlineData("6bbdc92f-3fea-4e00-9cb3-7b09df9864c7", false)]
    [InlineData("bf9c469b-980d-4349-8283-24aaf121c459", false)]
    [InlineData("e0f36e9c-c7f0-442d-a19d-ea1103e72dc7", false)]
    [InlineData("00000000-0000-0000-0000-000000000000", true)]
    public void IsEmpty_WhenCalled_ThenProperResultReturned(string guid, bool expected)
    {
        Guid.Parse(guid).IsEmpty().Should().Be(expected);
    }
}
