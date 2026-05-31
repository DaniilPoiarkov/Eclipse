# Eclipse MCP Server

An MCP (Model Context Protocol) server for the [Eclipse](https://github.com/DaniilPoiarkov/Eclipse) Telegram bot platform.
Exposes Eclipse's API as AI-callable tools, enabling assistants like Claude to manage todo items, reminders, mood records, users, commands, and more.

Built with the [ModelContextProtocol C# SDK](https://modelcontextprotocol.github.io/csharp-sdk) as a self-contained executable — no .NET runtime required on the target machine.

## Prerequisites

An `ECLIPSE_API_TOKEN` is required to authenticate with the Eclipse API. Set it as an environment variable when configuring the server.

## Tools

### Diagnostics

| Tool | Description |
|------|-------------|
| `eclipse_ping` | Checks minimal availability of the Eclipse app |
| `eclipse_health` | Calls the health check endpoint and returns application health status |

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

### Users *(admin)*

| Tool | Description |
|------|-------------|
| `eclipse_users_get_list` | Retrieves a paginated, filterable list of registered users |
| `eclipse_user_statistics_get` | Retrieves activity statistics (finished todos, received reminders) |

### Commands *(admin)*

| Tool | Description |
|------|-------------|
| `eclipse_commands_get_list` | Lists all registered bot commands |
| `eclipse_commands_add` | Registers a new bot command |
| `eclipse_commands_remove` | Removes a bot command |

### Feedbacks *(admin)*

| Tool | Description |
|------|-------------|
| `eclipse_feedbacks_get_list` | Retrieves a paginated list of user feedback |
| `eclipse_feedbacks_request` | Sends a feedback request to a user |

### Promotions *(admin)*

| Tool | Description |
|------|-------------|
| `eclipse_promotions_find` | Looks up a promotion by its slug |
| `eclipse_promotions_publish` | Publishes a promotion to users |

### Suggestions *(admin)*

| Tool | Description |
|------|-------------|
| `eclipse_suggestions_get_list` | Retrieves a paginated list of user suggestions |

### Telegram *(admin)*

| Tool | Description |
|------|-------------|
| `eclipse_telegram_send` | Sends a direct Telegram message to a chat by ID |
| `eclipse_telegram_switch_handler` | Switches the active Telegram webhook handler (`Active` / `Disabled`) |

### Configuration *(admin)*

| Tool | Description |
|------|-------------|
| `eclipse_configuration_get_cultures` | Lists available localization cultures |

### Cache *(admin)*

| Tool | Description |
|------|-------------|
| `eclipse_cache_prune` | Prunes the application cache |

### Inbox Messages *(admin)*

| Tool | Description |
|------|-------------|
| `eclipse_inbox_messages_reset_failed` | Resets failed inbox messages for reprocessing |

## Configuration

### Claude Desktop

Add to `claude_desktop_config.json`:

```json
{
  "mcpServers": {
    "eclipse": {
      "command": "dnx",
      "args": ["Eclipse", "--version", "0.1.5-beta", "--yes"],
      "env": {
        "ECLIPSE_API_TOKEN": "<your-token>"
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
      "args": ["Eclipse", "--version", "0.1.5-beta", "--yes"],
      "env": {
        "ECLIPSE_API_TOKEN": "<your-token>"
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
      "args": ["Eclipse", "--version", "0.1.5-beta", "--yes"],
      "env": {
        "ECLIPSE_API_TOKEN": "<your-token>"
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
        "ECLIPSE_API_TOKEN": "<your-token>"
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
dotnet nuget push mcp/Eclipse.MCP/nupkg/Eclipse.0.1.5-beta.nupkg --api-key <key> --source https://api.nuget.org/v3/index.json
```

## Supported Platforms

The package ships self-contained binaries for:

- `win-x64`, `win-arm64`
- `osx-arm64`
- `linux-x64`, `linux-arm64`, `linux-musl-x64`

## Links

- [Eclipse Repository](https://github.com/DaniilPoiarkov/Eclipse)
- [MCP C# SDK](https://modelcontextprotocol.github.io/csharp-sdk)
- [Model Context Protocol](https://modelcontextprotocol.io/)
- [NuGet Publishing Guide](https://aka.ms/nuget/mcp/guide)
