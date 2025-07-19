namespace Eclipse.Common.Plots;

public sealed class PlotOptions<TXAxes, TYAxes>
{
    public AxisOptions<TXAxes>? Bottom { get; set; }
    public AxisOptions<TYAxes>? Left { get; set; }

    public string? Title { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}
