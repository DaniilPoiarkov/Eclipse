using System.Diagnostics;

namespace Eclipse.Common.Tests.Specifications.Utils;

[DebuggerDisplay("X = {X}; Y = {Y}")]
internal sealed class TestObject
{
    public int X { get; set; }
    public int Y { get; set; }
}
