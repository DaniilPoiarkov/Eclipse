using ScottPlot;

namespace Eclipse.Infrastructure.Plots.Templates;

internal sealed class SansDarkTemplate : IPlotTemplate
{
    private static readonly Color _dataBackgroundColor = Color.FromHex("#0b3049");

    private static readonly Color _figureColor = Color.FromHex("#07263b");

    private static readonly Color _axesColor = Color.FromHex("#a0acb5");

    private static readonly string _font = "Sans Font";

    private static readonly float _minorTickSize = 1f;

    public Plot Create()
    {
        var plot = new Plot();

        plot.DataBackground.Color = _dataBackgroundColor;
        plot.FigureBackground.Color = _figureColor;

        plot.Axes.Color(_axesColor);

        plot.Axes.Bottom.MinorTickStyle.Length = _minorTickSize;
        plot.Axes.Left.MinorTickStyle.Length = _minorTickSize;

        plot.Font.Set(_font);

        return plot;
    }
}
