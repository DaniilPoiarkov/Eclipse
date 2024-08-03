using Eclipse.Common.Results;

namespace Eclipse.Tests.Utils;

public static class ErrorComparer
{
    public static bool AreEqual(Error left, Error right)
    {
        return left.Code == right.Code
            && left.Description == right.Description
            && left.Args.SequenceEqual(right.Args);
    }
}
