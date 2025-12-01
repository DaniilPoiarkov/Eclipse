using ScottPlot;
using ScottPlot.TickGenerators;

namespace Eclipse.Infrastructure.Plots.TickGenerators;

internal sealed class DateTimeTickGeneratorFactory : ITickGeneratorFactory<DateTime>
{
    private static readonly int _maxTicks = 11;

    private static readonly int[] _intervals = [1, 2, 3];

    public ITickGenerator Create(IList<DateTime> values)
    {
        var manual = new DateTimeManual()
        {
            MaxTickCount = _maxTicks
        };

        if (values.IsNullOrEmpty())
        {
            return manual;
        }

        var start = values.Min();
        var end = values.Max();

        var range = (end - start).TotalDays;

        var interval = _intervals.FirstOrDefault(i => range / i <= _maxTicks);

        if (interval == 0)
        {
            interval = (int)Math.Ceiling(range / _maxTicks);
        }

        var chunks = NormalizeDates(start, end)
            .Chunk(interval);

        foreach (var dates in chunks)
        {
            var first = dates[0];
            manual.AddMajor(first, $"{first:dd.MM}");

            for (int i = 1; i < dates.Length; i++)
            {
                manual.AddMinor(dates[i]);
            }
        }

        return manual;
    }

    private IEnumerable<DateTime> NormalizeDates(DateTime start, DateTime end)
    {
        var current = start.Date;

        yield return current;

        while (current < end)
        {
            current = current.NextDay();
            yield return current;
        }

        if (current != end.Date)
        {
            yield return end.Date;
        }
    }
}
