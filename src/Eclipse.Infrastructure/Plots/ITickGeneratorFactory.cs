using ScottPlot;

namespace Eclipse.Infrastructure.Plots;

internal interface ITickGeneratorFactory<T>
{
    ITickGenerator Create(IList<T> values);
}
