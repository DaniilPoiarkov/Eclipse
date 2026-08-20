using ScottPlot;

namespace Eclipse.Infrastructure.Plots.Templates;

internal sealed class SansDarkTemplate : IPlotTemplate
{
    private static readonly Color _dataBackgroundColor = Color.FromHex("#0b3049");

    private static readonly Color _figureColor = Color.FromHex("#07263b");

    private static readonly Color _axesColor = Color.FromHex("#a0acb5");

    public Plot Create()
    {
        var plot = new Plot();

        plot.DataBackground.Color = _dataBackgroundColor;
        plot.FigureBackground.Color = _figureColor;

        plot.Axes.Color(_axesColor);

        plot.Axes.Bottom.MinorTickStyle.Length = TemplateConsts.MinorTickSize;
        plot.Axes.Left.MinorTickStyle.Length = TemplateConsts.MinorTickSize;

        plot.Font.Set(TemplateConsts.SansFont);

        return plot;
    }
}
