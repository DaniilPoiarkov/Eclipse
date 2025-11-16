using ScottPlot;
using ScottPlot.TickGenerators;

namespace Eclipse.Infrastructure.Plots.TickGenerators;

internal sealed class DateTimeTickGeneratorFactory : ITickGeneratorFactory<DateTime>
{
    public ITickGenerator Create(IList<DateTime> values)
    {
        var manual = new DateTimeManual();

        var intervals = values.Count switch
        {
            28 or 30 => 3,
            >= 21 and <= 29 or 31 => 2,
            _ => 1,
        };

        var chunks = values.Select(v => v.WithTime(0, 0))
            .Chunk(intervals);

        if (chunks.IsNullOrEmpty())
        {
            return manual;
        }

        foreach (var date in chunks.Select(c => c.First()))
        {
            manual.AddMajor(date, $"{date:dd.MM}");
        }

        var last = chunks.Last();

        if (last.Length > 1)
        {
            var date = last[^1];
            manual.AddMajor(date, $"{date:dd.MM}");
        }

        return manual;
    }
}
