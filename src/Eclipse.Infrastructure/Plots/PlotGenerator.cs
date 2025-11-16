using Eclipse.Common.Plots;

using ScottPlot;
using ScottPlot.TickGenerators;

namespace Eclipse.Infrastructure.Plots;

internal sealed class PlotGenerator : IPlotGenerator
{
    private static readonly int _tickInterval = 1;

    private readonly ITickGeneratorFactory<DateTime> _dateTimeTickGeneratorFactory;

    private readonly IPlotTemplateFactory _plotTemplateFactory;

    public PlotGenerator(ITickGeneratorFactory<DateTime> dateTimeTickGeneratorFactory, IPlotTemplateFactory plotTemplateFactory)
    {
        _dateTimeTickGeneratorFactory = dateTimeTickGeneratorFactory;
        _plotTemplateFactory = plotTemplateFactory;
    }

    public MemoryStream Create<TYAxis>(PlotOptions<DateTime, TYAxis> options)
    {
        var xValues = options.Bottom?.Values ?? [];
        var yValues = options.Left?.Values ?? [];

        using var plot = _plotTemplateFactory.Create(PlotTemplateType.SegoeDark).Create();

        plot.Axes.Bottom.TickGenerator = _dateTimeTickGeneratorFactory.Create(xValues);
        plot.Axes.Left.TickGenerator = new NumericFixedInterval(_tickInterval);

        FillPlotTitles(plot, options);

        plot.Add.Scatter(xValues, yValues);

        var bytes = plot.GetImageBytes(options.Width, options.Height, ImageFormat.Png);

        return new MemoryStream(bytes);
    }

    private static void FillPlotTitles<TXAxis, TYAxis>(Plot plot, PlotOptions<TXAxis, TYAxis> options)
    {
        if (!options.Title.IsNullOrEmpty())
        {
            plot.Title(options.Title);
        }

        FillAxis(options.Bottom, plot.Axes.Bottom);
        FillAxis(options.Left, plot.Axes.Left);
    }

    private static void FillAxis<T>(AxisOptions<T>? options, IAxis axis)
    {
        if (options is null)
        {
            return;
        }

        axis.Min = options.Min;
        axis.Max = options.Max;

        axis.Label.Text = options.Label;
        axis.Label.IsVisible = !options.Label.IsNullOrEmpty();
    }
}
