using Eclipse.Common.Results;

using FluentAssertions;

namespace Eclipse.Tests.Utils;

public static class ErrorComparer
{
    public static void AreEqual(Error left, Error right)
    {
        var result = left.Code == right.Code
            && left.Description == right.Description;

        result.Should().BeTrue();
        left.Args.Should().BeEquivalentTo(right.Args);
    }
}
