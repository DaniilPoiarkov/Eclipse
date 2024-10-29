namespace Eclipse.Common.Plots;

public interface IPlotGenerator
{
    MemoryStream Create<TYAxis>(PlotOptions<DateTime, TYAxis> options);
}
