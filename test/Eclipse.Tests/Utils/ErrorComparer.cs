using Eclipse.Common.Results;

using FluentAssertions;

namespace Eclipse.Tests.Utils;

public static class ErrorComparer
{
    public static void AreEqual(Error left, Error right)
    {
        left.Code.Should().Be(right.Code);
        left.Description.Should().Be(right.Description);
        left.Args.Should().BeEquivalentTo(right.Args);
        left.Type.Should().Be(right.Type);
    }
}
