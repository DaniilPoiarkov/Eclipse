namespace Eclipse.Infrastructure.Plots;

internal interface IPlotTemplateFactory
{
    IPlotTemplate Create(PlotTemplateType type);
}
