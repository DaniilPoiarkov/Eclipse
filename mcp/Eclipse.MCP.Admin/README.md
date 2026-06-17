# Eclipse Admin MCP Server

An MCP (Model Context Protocol) server for the [Eclipse](https://github.com/DaniilPoiarkov/Eclipse) Telegram bot platform.
Exposes Eclipse's admin API as AI-callable tools, enabling assistants like Claude to manage users, commands, feedbacks, promotions, and platform internals.

Built with the [ModelContextProtocol C# SDK](https://modelcontextprotocol.github.io/csharp-sdk) as a self-contained executable — no .NET runtime required on the target machine.

> **Admin access required.** All tools in this server require an API token with admin privileges.

## Prerequisites

Two environment variables are required when configuring the server:

- `ECLIPSE_API_TOKEN` — Admin API token for authenticating with the Eclipse API.
- `ECLIPSE_MODE` — Selects the target environment. Must be the string `"STANDARD"` for production. Set to `"TESTING"` only when connecting to the development environment.

## Tools

### Diagnostics

| Tool | Description |
|------|-------------|
| `eclipse_ping` | Checks minimal availability of the Eclipse app |
| `eclipse_health` | Calls the health check endpoint and returns application health status |

### Users

| Tool | Description |
|------|-------------|
| `eclipse_users_get_list` | Retrieves a paginated, filterable list of registered users |

### Commands

| Tool | Description |
|------|-------------|
| `eclipse_commands_get_list` | Lists all registered Telegram bot commands |
| `eclipse_commands_add` | Registers a new bot command |
| `eclipse_commands_remove` | Removes a bot command |

### Feedbacks

| Tool | Description |
|------|-------------|
| `eclipse_feedbacks_get_list` | Retrieves a paginated list of user feedback |
| `eclipse_feedbacks_request` | Sends a feedback request to all users |

### Promotions

| Tool | Description |
|------|-------------|
| `eclipse_promotions_find` | Looks up a promotion by its ID |
| `eclipse_promotions_publish` | Publishes a promotion to users |

### Suggestions

| Tool | Description |
|------|-------------|
| `eclipse_suggestions_get_list` | Retrieves all user suggestions with submitter information |

### Telegram

| Tool | Description |
|------|-------------|
| `eclipse_telegram_send` | Sends a direct Telegram message to a chat by ID |
| `eclipse_telegram_switch_handler` | Switches the active Telegram webhook handler (`Active` / `Disabled`) |

### Cache

| Tool | Description |
|------|-------------|
| `eclipse_cache_prune` | Prunes the application cache |

### Inbox Messages

| Tool | Description |
|------|-------------|
| `eclipse_inbox_messages_reset_failed` | Resets failed inbox messages for reprocessing |

## Configuration

### Claude Desktop

Add to `claude_desktop_config.json`:

```json
{
  "mcpServers": {
    "eclipse-admin": {
      "command": "dnx",
      "args": ["DaniilP.Eclipse.Admin", "--version", "<version>", "--yes"],
      "env": {
        "ECLIPSE_API_TOKEN": "<your-admin-token>",
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
    "eclipse-admin": {
      "type": "stdio",
      "command": "dnx",
      "args": ["DaniilP.Eclipse.Admin", "--version", "<version>", "--yes"],
      "env": {
        "ECLIPSE_API_TOKEN": "<your-admin-token>",
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
    "eclipse-admin": {
      "type": "stdio",
      "command": "dnx",
      "args": ["DaniilP.Eclipse.Admin", "--version", "<version>", "--yes"],
      "env": {
        "ECLIPSE_API_TOKEN": "<your-admin-token>",
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
    "eclipse-admin": {
      "type": "stdio",
      "command": "dotnet",
      "args": ["run", "--project", "<path-to>/mcp/Eclipse.MCP.Admin"],
      "env": {
        "ECLIPSE_API_TOKEN": "<your-admin-token>",
        "ECLIPSE_MODE": "STANDARD"
      }
    }
  }
}
```

## Publishing

```powershell
dotnet pack mcp/Eclipse.MCP.Admin/Eclipse.MCP.Admin.csproj -c Release -o mcp/Eclipse.MCP.Admin/nupkg
dotnet nuget push mcp/Eclipse.MCP.Admin/nupkg/DaniilP.Eclipse.Admin.<version>.nupkg --api-key <key> --source https://api.nuget.org/v3/index.json
```

## Supported Platforms

The package ships self-contained binaries for:

- `win-x64`, `win-arm64`
- `osx-arm64`
- `linux-x64`, `linux-arm64`, `linux-musl-x64`

## Links

- [Eclipse Repository](https://github.com/DaniilPoiarkov/Eclipse)
