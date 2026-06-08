using Eclipse.MCP.Admin.Tools;
using Eclipse.MCP.Core.Client;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>();

// Configure all logs to go to stderr (stdout is used for the MCP protocol messages).
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

builder.Services.AddOptions<EclipseOptions>()
    .Bind(builder.Configuration.GetSection("Eclipse"))
    .ValidateOnStart();

builder.Services.AddHttpClient<IEclipseClient, EclipseClient>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<EclipseOptions>>();
    var configuration = sp.GetRequiredService<IConfiguration>();

    var url = configuration["ECLIPSE_MODE"] == "TESTING"
        ? options.Value.TestingUrl
        : options.Value.StandardUrl;

    client.BaseAddress = new Uri(url);
    client.DefaultRequestHeaders.Add("X-Api-Token", configuration["ECLIPSE_API_TOKEN"]);
});

builder.Services.AddScoped<CacheTools>()
    .AddScoped<CommandTools>()
    .AddScoped<FeedbackTools>()
    .AddScoped<HealthTools>()
    .AddScoped<InboxMessageTools>()
    .AddScoped<PingTools>()
    .AddScoped<PromotionTools>()
    .AddScoped<SuggestionTools>()
    .AddScoped<TelegramTools>()
    .AddScoped<UserTools>();

builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<CacheTools>()
    .WithTools<CommandTools>()
    .WithTools<FeedbackTools>()
    .WithTools<HealthTools>()
    .WithTools<InboxMessageTools>()
    .WithTools<PingTools>()
    .WithTools<PromotionTools>()
    .WithTools<SuggestionTools>()
    .WithTools<TelegramTools>()
    .WithTools<UserTools>();

await builder.Build().RunAsync();
