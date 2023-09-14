using Eclipse.Application;
using Eclipse.Application.Contracts;
using Eclipse.Core;
using Eclipse.DataAccess;
using Eclipse.Domain;
using Eclipse.Domain.Shared;
using Eclipse.Infrastructure;
using Eclipse.Infrastructure.Builder;
using Eclipse.Infrastructure.Telegram;
using Eclipse.Pipelines;
using Eclipse.Pipelines.UpdateHandler;
using Eclipse.WebAPI;
using Eclipse.WebAPI.Filters;
using Eclipse.WebAPI.Middlewares;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services
    .AddApplicationModule()
    .AddDomainSharedModule()
    .AddDomainModule()
    .AddCoreModule()
    .AddApplicationContractsModule()
    .AddPipelinesModule()
    .AddWebApiModule()
    .AddDataAccessModule()
    .AddInfrastructureModule(config =>
    {
        config.TelegramOptions = new TelegramOptions
        {
            Token = configuration["Telegram:Token"]!,
            Chat = long.Parse(configuration["Telegram:Chat"]!),
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

builder.Services.Configure<ApiKeyAuthorizationOptions>(
    builder.Configuration.GetSection("Authorization")
);

builder.Host.UseSerilog((_, config) =>
{
    config.WriteTo.Console();
});

var app = builder.Build();

var starter = app.Services.GetRequiredService<IEclipseStarter>();
await starter.StartAsync();

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    ///
}

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
