namespace Eclipse.Core.Routing;

[AttributeUsage(AttributeTargets.Class)]
public sealed class RouteAttribute : Attribute
{
    public string Route { get; }

    public string? Command { get; }

    public RouteAttribute(string route, string? command = null)
    {
        Route = route;
        Command = command;
    }
}
