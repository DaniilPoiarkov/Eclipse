using System.Collections;

namespace Eclipse.Common.Tests.Extensions.TestData;

internal sealed class ToDateTimeTestData : IEnumerable<object?[]>
{
    public IEnumerator<object?[]> GetEnumerator()
    {
        yield return [
            null, default
        ];

        yield return [
            new DateTime(2024, 10, 1), new DateTime(2024, 10, 1),
        ];

        yield return [
            new DateTime(2024, 9, 9).ToString(), new DateTime(2024, 9, 9)
        ];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
