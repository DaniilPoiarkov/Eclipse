using System.Collections;

namespace Eclipse.Common.Tests.Extensions.TestData;

internal sealed class JoinTestData : IEnumerable<object?[]>
{
    public IEnumerator<object?[]> GetEnumerator()
    {
        yield return [
            new string[] { "x", "y", "z" },
            ", ",
            "x, y, z"
        ];

        yield return [
            Array.Empty<string>(),
            ", ",
            ""
        ];

        yield return [
            new string[] { "x", "y", "z" },
            "-",
            "x-y-z"
        ];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
