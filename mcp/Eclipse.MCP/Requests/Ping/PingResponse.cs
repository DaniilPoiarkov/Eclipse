namespace Eclipse.MCP.Requests.Ping;

public record struct PingResponse(string Message, bool IsError);
