namespace Eclipse.Common.Plots;

public sealed class AxisOptions<TValue>
{
    public TValue[] Values { get; set; } = [];
    public string Label { get; set; } = string.Empty;
    public double Min { get; set; }
    public double Max { get; set; }
}
