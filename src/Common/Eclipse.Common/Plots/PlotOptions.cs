namespace Eclipse.Common.Plots;

public sealed class PlotOptions<TXAxes, TYAxes>
{
    public TXAxes[] Xs { get; set; } = [];
    public TYAxes[] Ys { get; set; } = [];

    public string? Title { get; set; }
    public string? YAxisTitle { get; set; }
    public string? XAxisTitle { get; set; }

    public int Width { get; set; }
    public int Height { get; set; }
}
