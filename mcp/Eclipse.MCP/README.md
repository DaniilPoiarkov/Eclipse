# Eclipse MCP Server

An MCP (Model Context Protocol) server for the [Eclipse](https://github.com/DaniilPoiarkov/Eclipse) Telegram bot platform.
Exposes Eclipse's user-facing API as AI-callable tools, enabling assistants like Claude to manage todo items, reminders, mood records, and more.

Built with the [ModelContextProtocol C# SDK](https://modelcontextprotocol.github.io/csharp-sdk) as a self-contained executable — no .NET runtime required on the target machine.

## Prerequisites

Two environment variables are required when configuring the server:

- `ECLIPSE_API_TOKEN` — API token for authenticating with the Eclipse API.
- `ECLIPSE_MODE` — Selects the target environment. Must be the string `"STANDARD"` for production. Set to `"TESTING"` only when connecting to the development environment.

## Tools

### Todo Items

| Tool | Description |
|------|-------------|
| `eclipse_todo_items_get_list` | Retrieves all todo items for the authenticated user |
| `eclipse_todo_items_get` | Retrieves a specific todo item by ID |
| `eclipse_todo_items_add` | Creates a new todo item |
| `eclipse_todo_items_finish` | Marks a todo item as finished |

### Reminders

| Tool | Description |
|------|-------------|
| `eclipse_reminders_get_list` | Retrieves all reminders for the authenticated user |
| `eclipse_reminders_get` | Retrieves a specific reminder by ID |
| `eclipse_reminders_create` | Creates a new reminder with a notification time |

### Mood Records

| Tool | Description |
|------|-------------|
| `eclipse_mood_records_get_list` | Retrieves mood records for the authenticated user |
| `eclipse_mood_records_get` | Retrieves a specific mood record by ID |
| `eclipse_mood_records_add` | Creates or updates the mood record for the current day |

### Configuration

| Tool | Description |
|------|-------------|
| `eclipse_configuration_get_cultures` | Lists available localization cultures |

### Statistics

| Tool | Description |
|------|-------------|
| `eclipse_user_statistics_get` | Retrieves activity statistics (finished todos, received reminders) |

## Configuration

### Claude Desktop

Add to `claude_desktop_config.json`:

```json
{
  "mcpServers": {
    "eclipse": {
      "command": "dnx",
      "args": ["DaniilP.Eclipse", "--version", "<version>", "--yes"],
      "env": {
        "ECLIPSE_API_TOKEN": "<your-token>",
        "ECLIPSE_MODE": "STANDARD"
      }
    }
  }
}
```

### VS Code

Create `.vscode/mcp.json` in your workspace:

```json
{
  "servers": {
    "eclipse": {
      "type": "stdio",
      "command": "dnx",
      "args": ["DaniilP.Eclipse", "--version", "<version>", "--yes"],
      "env": {
        "ECLIPSE_API_TOKEN": "<your-token>",
        "ECLIPSE_MODE": "STANDARD"
      }
    }
  }
}
```

### Visual Studio

Create `.mcp.json` in your solution directory:

```json
{
  "servers": {
    "eclipse": {
      "type": "stdio",
      "command": "dnx",
      "args": ["DaniilP.Eclipse", "--version", "<version>", "--yes"],
      "env": {
        "ECLIPSE_API_TOKEN": "<your-token>",
        "ECLIPSE_MODE": "STANDARD"
      }
    }
  }
}
```

### Run from source

```json
{
  "servers": {
    "eclipse": {
      "type": "stdio",
      "command": "dotnet",
      "args": ["run", "--project", "<path-to>/mcp/Eclipse.MCP"],
      "env": {
        "ECLIPSE_API_TOKEN": "<your-token>",
        "ECLIPSE_MODE": "STANDARD"
      }
    }
  }
}
```

## Publishing

Use the provided script to pack and push to NuGet.org:

```powershell
.\scripts\pack-and-publish-mcp.ps1 -ApiKey <your-nuget-api-key>
```

Or manually:

```powershell
dotnet pack mcp/Eclipse.MCP/Eclipse.MCP.csproj -c Release -o mcp/Eclipse.MCP/nupkg
dotnet nuget push mcp/Eclipse.MCP/nupkg/DaniilP.Eclipse.<version>.nupkg --api-key <key> --source https://api.nuget.org/v3/index.json
```

## Supported Platforms

The package ships self-contained binaries for:

- `win-x64`, `win-arm64`
- `osx-arm64`
- `linux-x64`, `linux-arm64`, `linux-musl-x64`

## Links

- [Eclipse Repository](https://github.com/DaniilPoiarkov/Eclipse)
