namespace Eclipse.Common.Plots;

public sealed class AxisOptions<TValue>
{
    public TValue[] Values { get; set; } = [];
    public string Label { get; set; } = string.Empty;
    public int Min { get; set; }
    public int Max { get; set; }
}
