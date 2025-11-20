using Eclipse.Infrastructure.Plots.Templates;

namespace Eclipse.Infrastructure.Plots;

internal sealed class PlotTemplateFactory : IPlotTemplateFactory
{
    public IPlotTemplate Create(PlotTemplateType type)
    {
        return type switch
        {
            PlotTemplateType.SansDark => new SansDarkTemplate(),
            _ => throw new NotSupportedException($"Plot template type '{type}' is not supported.")
        };
    }
}
