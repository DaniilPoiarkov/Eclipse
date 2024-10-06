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

    private static readonly string _font = "Montresat";

    private static readonly int _minorTickSize = 0;

    private static readonly int _tickInterval = 1;

    public MemoryStream Create<TYAxis>(PlotOptions<DateTime, TYAxis> options)
    {
        var yAxisTickGenerator = new NumericFixedInterval(_tickInterval);

        var xAxisTickGenerator = new DateTimeFixedInterval(new Day())
        {
            LabelFormatter = date => $"{date:dd.MM}",
            GetIntervalStartFunc = dt => dt.WithTime(0, 0),
        };

        using var plot = GetTemplate(xAxisTickGenerator, yAxisTickGenerator);

        FillPlotTitles(plot, options);

        plot.Add.Scatter(options.Xs.ToArray(), options.Ys.ToArray());

        var bytes = plot.GetImageBytes(options.Width, options.Height, ImageFormat.Png);

        return new MemoryStream(bytes);
    }

    private static void FillPlotTitles<TXAxis, TYAxis>(Plot plot, PlotOptions<TXAxis, TYAxis> options)
    {
        if (!options.Title.IsNullOrEmpty())
        {
            plot.Title(options.Title);
        }
        if (!options.XAxisTitle.IsNullOrEmpty())
        {
            plot.XLabel(options.XAxisTitle);
        }
        if (!options.YAxisTitle.IsNullOrEmpty())
        {
            plot.YLabel(options.YAxisTitle);
        }
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
