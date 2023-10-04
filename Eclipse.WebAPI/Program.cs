using Eclipse.Application;
using Eclipse.Application.Contracts;
using Eclipse.Core;
using Eclipse.DataAccess;
using Eclipse.DataAccess.CosmosDb;
using Eclipse.Domain;
using Eclipse.Domain.Shared;
using Eclipse.Infrastructure;
using Eclipse.Infrastructure.Builder;
using Eclipse.Localization;
using Eclipse.Pipelines;
using Eclipse.Pipelines.Decorations;
using Eclipse.Pipelines.UpdateHandler;
using Eclipse.WebAPI;
using Eclipse.WebAPI.Filters;
using Eclipse.WebAPI.HealthChecks;
using Eclipse.WebAPI.Middlewares;

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
    .AddDataAccessModule(builder =>
    {
        builder.CosmosOptions = configuration.GetSection("Azure:CosmosDb")
            .Get<CosmosDbContextOptions>()!;
    });

builder.Services
    .AddInfrastructureModule(config =>
    {
        config.TelegramOptions = configuration.GetSection("Telegram")
            .Get<TelegramOptions>();

        config.UseTelegramHandler<ITelegramUpdateHandler>();

        config.CacheOptions = new CacheOptions
        {
            Expiration = new TimeSpan(1, 0, 0, 0)
        };

        config.GoogleOptions = new GoogleOptions
        {
            Credentials = configuration["Google:Credentials"]!
        };
    });

builder.Services.AddLocalizationSupport(builder =>
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

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseEclipseHealthCheks();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
