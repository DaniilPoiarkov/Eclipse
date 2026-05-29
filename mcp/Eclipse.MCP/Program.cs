using Eclipse.MCP;
using Eclipse.MCP.Tools;
using Eclipse.MCP.Tools.Ping;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
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

    client.BaseAddress = new Uri(options.Value.Url);
    client.DefaultRequestHeaders.Add("X-MCP-Token", configuration["ECLIPSE_API_TOKEN"]);
});

builder.Services.AddScoped<PingTools>();
builder.Services.AddScoped<TestTools>();

builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<PingTools>()
    .WithTools<TestTools>();

await builder.Build().RunAsync();
