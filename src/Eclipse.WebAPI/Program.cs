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
