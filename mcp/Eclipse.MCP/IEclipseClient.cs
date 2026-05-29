using Eclipse.MCP.Tools;

namespace Eclipse.MCP;

public interface IEclipseClient
{
    Task<PingResponse> PingAsync();
}
