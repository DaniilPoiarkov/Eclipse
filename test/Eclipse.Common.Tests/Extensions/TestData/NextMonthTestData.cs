using System.Collections;

namespace Eclipse.Common.Tests.Extensions.TestData;

internal sealed class NextMonthTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return [
            new DateTime(2024, 9, 30, 23, 59, 59), new DateTime(2024, 10, 1, 0, 0, 0)
        ];

        yield return [
            new DateTime(2024, 12, 30, 0, 0, 0), new DateTime(2025, 1, 1, 0, 0, 0)
        ];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
