using Eclipse.Application;
using Eclipse.Application.Contracts;
using Eclipse.Application.Contracts.Users;
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
using Eclipse.WebAPI.Filters.Authorization;
using Eclipse.WebAPI.Health;
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
    .AddDataAccessModule(options => configuration.GetSection("Azure").Bind(options));

builder.Services
    .AddInfrastructureModule()
    .UseTelegramHandler<IEclipseUpdateHandler>()
    .ConfigureCacheOptions(options => options.Expiration = TimeSpan.FromDays(3))
    .ConfigureGoogleOptions(options => configuration.GetSection("Google").Bind(options))
    .ConfigureTelegramOptions(options => configuration.GetSection("Telegram").Bind(options));

builder.Services.AddLocalization(localization =>
{
    var options = configuration.GetSection("Localization")
        .Get<LocalizationOptions>()!;

    var path = "EmbeddedResources/Localizations";

    foreach (var culture in options.AvailableCultures)
    {
        localization.AddJsonFiles($"{path}/{culture}");
    }

    localization.DefaultLocalization = options.DefaultCulture;
});

builder.Services.Configure<ApiKeyAuthorizationOptions>(
    configuration.GetSection("Authorization")
);

builder.Host.UseSerilog((_, config) =>
{
    config.WriteTo.Console();
});

var app = builder.Build();

//using var scope = app.Services.CreateScope();

//var users = await scope.ServiceProvider.GetRequiredService<IUserService>().GetAllAsync();

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    ///
}

app.UseExceptionHandler()
    .UseHttpsRedirection();

app.UseEclipseHealthChecks();

app.UseAuthentication()
    .UseAuthorization();

app.MapControllers();

await app.InitializePipelineModuleAsync();

app.Run();

public partial class Program;
