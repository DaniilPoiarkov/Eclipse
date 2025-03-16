using Eclipse.Application;
using Eclipse.Core;
using Eclipse.DataAccess;
using Eclipse.Domain;
using Eclipse.Infrastructure;
using Eclipse.Localization;
using Eclipse.Pipelines;
using Eclipse.Pipelines.Decorations;
using Eclipse.WebAPI;
using Eclipse.WebAPI.Options;

using Microsoft.ApplicationInsights.Extensibility;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services
    .AddApplicationModule(options => configuration.GetSection("Application").Bind(options))
    .AddDomainModule()
    .AddCoreModule(builder => builder.Decorate<LocalizationDecorator>())
    .AddPipelinesModule(options => configuration.GetSection("Telegram").Bind(options))
    .AddWebApiModule()
    .AddDataAccessModule()
    .ApplyMigrations(typeof(EclipseDataAccessModule).Assembly)
    .AddInfrastructureModule();

builder.Services.AddLocalization(localization =>
{
    var options = configuration.GetSection("Localization")
        .Get<LocalizationOptions>()!;

    var path = "EmbeddedResources/Localizations";

    foreach (var culture in options.AvailableCultures)
    {
        localization.AddJsonFiles($"{path}/{culture}");
    }

    localization.DefaultCulture = options.DefaultCulture;
});

builder.Host.UseSerilog((context, sp, config) =>
{
    config.ReadFrom.Configuration(configuration)
        .WriteTo.Async(sink => sink.Console())
        .WriteTo.Async(sink => sink.ApplicationInsights(
            sp.GetRequiredService<TelemetryConfiguration>(),
            TelemetryConverter.Traces))
        .Enrich.FromLogContext();
});

var app = builder.Build();

app.InitializeWebApiModule();

await app.InitializeApplicationLayerAsync();
await app.InitializeDataAccessModuleAsync();
await app.InitializePipelineModuleAsync();

await app.RunAsync();

public partial class Program;
