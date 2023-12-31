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
using Eclipse.Pipelines.UpdateHandler;
using Eclipse.WebAPI;
using Eclipse.WebAPI.Filters;
using Eclipse.WebAPI.HealthChecks;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services
    .AddEclipseHealthChecks()
    .AddApplicationModule()
    .AddDomainSharedModule()
    .AddDomainModule()
    .AddCoreModule(builder => builder.Decorate<LocalizationDecorator>())
    .AddApplicationContractsModule()
    .AddPipelinesModule()
    .AddWebApiModule()
    .AddDataAccessModule(options => configuration.GetSection("Azure").Bind(options));

builder.Services
    .AddInfrastructureModule()
    .UseTelegramHandler<ITelegramUpdateHandler>()
    .ConfigureCacheOptions(options => options.Expiration = TimeSpan.FromDays(3))
    .ConfigureGoogleOptions(options => configuration.GetSection("Google").Bind(options))
    .ConfigureTelegramOptions(options => configuration.GetSection("Telegram").Bind(options));

builder.Services.AddLocalization(builder =>
{
    var path = "EmbeddedResources/Localizations/";

    builder.AddJsonFiles($"{path}en")
        .AddJsonFiles($"{path}uk");

    builder.DefaultLocalization = "uk";
});

builder.Services.Configure<ApiKeyAuthorizationOptions>(
    builder.Configuration.GetSection("Authorization")
);

builder.Host.UseSerilog((_, config) =>
{
    config.WriteTo.Console();
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    ///
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseEclipseHealthCheks();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.InitializeDataAccessModule();
await app.InitializeApplicationModuleAsync();
await app.InitializePipelineModuleAsync();

app.Run();
