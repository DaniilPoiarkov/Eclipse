using Eclipse.Common.Plots;

using ScottPlot;
using ScottPlot.TickGenerators;
using ScottPlot.TickGenerators.TimeUnits;

namespace Eclipse.Infrastructure.Plots;

internal sealed class PlotGenerator : IPlotGenerator
{
    private static readonly Color _dataColor = Color.FromHex("#0b3049");

    private static readonly Color _figureColor = Color.FromHex("#07263b");

    private static readonly Color _axesColor = Color.FromHex("#a0acb5");

    private static readonly string _font = "Segoe UI";

    private static readonly int _minorTickSize = 0;

    private static readonly int _tickInterval = 1;

    public MemoryStream Create<TYAxis>(PlotOptions<DateTime, TYAxis> options)
    {
        var yAxisTickGenerator = new NumericFixedInterval(_tickInterval);

        var xAxisTickGenerator = new DateTimeFixedInterval(new Day())
        {
            LabelFormatter = date => $"{date:dd.MM}",
            GetIntervalStartFunc = date => date.WithTime(0, 0),
        };

        using var plot = GetTemplate(xAxisTickGenerator, yAxisTickGenerator);

        FillPlotTitles(plot, options);

        var xValues = options.Bottom?.Values ?? [];
        var yValues = options.Left?.Values ?? [];

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

    private static Plot GetTemplate(ITickGenerator xAxisTickGenerator, ITickGenerator yAxisTickGenerator)
    {
        var plot = new Plot();

        plot.Axes.Bottom.TickGenerator = xAxisTickGenerator;
        plot.Axes.Left.TickGenerator = yAxisTickGenerator;

        plot.DataBackground.Color = _dataColor;
        plot.FigureBackground.Color = _figureColor;

        plot.Axes.Color(_axesColor);

        plot.Axes.Bottom.MinorTickStyle.Length = _minorTickSize;
        plot.Axes.Left.MinorTickStyle.Length = _minorTickSize;

        plot.Font.Set(_font);

        return plot;
    }
}
