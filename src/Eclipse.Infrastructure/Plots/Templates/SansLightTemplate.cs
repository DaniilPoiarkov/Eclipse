using ScottPlot;

namespace Eclipse.Infrastructure.Plots.Templates;

internal sealed class SansLightTemplate : IPlotTemplate
{
    private static readonly Color _dataBackgroundColor = Color.FromHex("#CBF3BB");

    private static readonly Color _figureColor = Color.FromHex("#ABE7B2");

    private static readonly Color _axesColor = Color.FromHex("#93BFC7");

    public Plot Create()
    {
        var plot = new Plot();

        plot.DataBackground.Color = _dataBackgroundColor;
        plot.FigureBackground.Color = _figureColor;

        plot.Axes.Color(_axesColor);

        plot.Axes.Bottom.MinorTickStyle.Length = TemplateConsts.MinorTickSize;
        plot.Axes.Bottom.TickLabelStyle.ForeColor = _axesColor;

        plot.Axes.Left.MinorTickStyle.Length = TemplateConsts.MinorTickSize;
        plot.Axes.Left.TickLabelStyle.ForeColor = _axesColor;

        plot.Font.Set(TemplateConsts.SansFont);

        return plot;
    }
}
