using Eclipse.MCP.Core.Client;
using Eclipse.MCP.Tools;

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

    client.BaseAddress = new Uri(options.Value.Url);
    client.DefaultRequestHeaders.Add("X-MCP-Token", configuration["ECLIPSE_API_TOKEN"]);
});

builder.Services.AddScoped<PingTools>();
builder.Services.AddScoped<HealthTools>();
builder.Services.AddScoped<TodoItemTools>();
builder.Services.AddScoped<ReminderTools>();
builder.Services.AddScoped<MoodRecordTools>();
builder.Services.AddScoped<CacheTools>();
builder.Services.AddScoped<CommandTools>();
builder.Services.AddScoped<ConfigurationTools>();
builder.Services.AddScoped<FeedbackTools>();
builder.Services.AddScoped<InboxMessageTools>();
builder.Services.AddScoped<PromotionTools>();
builder.Services.AddScoped<SuggestionTools>();
builder.Services.AddScoped<TelegramTools>();
builder.Services.AddScoped<UserStatisticsTools>();
builder.Services.AddScoped<UserTools>();

builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<PingTools>()
    .WithTools<HealthTools>()
    .WithTools<TodoItemTools>()
    .WithTools<ReminderTools>()
    .WithTools<MoodRecordTools>()
    .WithTools<CacheTools>()
    .WithTools<CommandTools>()
    .WithTools<ConfigurationTools>()
    .WithTools<FeedbackTools>()
    .WithTools<InboxMessageTools>()
    .WithTools<PromotionTools>()
    .WithTools<SuggestionTools>()
    .WithTools<TelegramTools>()
    .WithTools<UserStatisticsTools>()
    .WithTools<UserTools>();

await builder.Build().RunAsync();
