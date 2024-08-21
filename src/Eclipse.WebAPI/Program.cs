using Eclipse.Application;
using Eclipse.Application.Contracts;
using Eclipse.Core;
using Eclipse.DataAccess;
using Eclipse.Domain;
using Eclipse.Domain.Shared;
using Eclipse.Infrastructure;
using Eclipse.Localization;
using Eclipse.Pipelines;
using Eclipse.Pipelines.Decorations;
using Eclipse.WebAPI;
using Eclipse.WebAPI.Options;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services
    .AddApplicationModule()
    .AddDomainSharedModule()
    .AddDomainModule()
    .AddCoreModule(builder => builder.Decorate<LocalizationDecorator>())
    .AddApplicationContractsModule()
    .AddPipelinesModule()
    .AddWebApiModule()
    .AddDataAccessModule()
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

builder.Host.UseSerilog((_, config) =>
{
    config.WriteTo.Async(sink => sink.Console());
});

var app = builder.Build();

app.InitializeWebApiModule();
await app.InitializaDataAccessModuleAsync();
await app.InitializePipelineModuleAsync();

app.Run();

public partial class Program;
