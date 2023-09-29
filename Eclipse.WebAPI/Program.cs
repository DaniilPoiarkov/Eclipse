using Eclipse.Application;
using Eclipse.Application.Contracts;
using Eclipse.Application.Extensions;
using Eclipse.Core;
using Eclipse.DataAccess;
using Eclipse.Domain;
using Eclipse.Domain.Shared;
using Eclipse.Infrastructure;
using Eclipse.Infrastructure.Builder;
using Eclipse.Localization;
using Eclipse.Pipelines;
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
    .AddCoreModule()
    .AddApplicationContractsModule()
    .AddPipelinesModule()
    .AddWebApiModule()
    .AddDataAccessModule(builder =>
    {
        builder.CosmosOptions.ConnectionString = configuration["Azure:CosmosDb:ConnectionString"]!;
        builder.CosmosOptions.DatabaseId = configuration["Azure:CosmosDb:DatabaseId"]!;
    });

builder.Services
    .AddInfrastructureModule(config =>
    {
        config.TelegramOptions = new TelegramOptions
        {
            Token = configuration["Telegram:Token"]!,
            Chat = configuration["Telegram:Chat"]!.ToLong(),
        };

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

    builder.AddJsonFile($"{path}en.json")
        .AddJsonFile($"{path}uk.json");

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
