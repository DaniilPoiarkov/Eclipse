using Eclipse.Application;
using Eclipse.Application.Contracts;
using Eclipse.Application.Contracts.Telegram;
using Eclipse.Core;
using Eclipse.Domain;
using Eclipse.Domain.Shared;
using Eclipse.Infrastructure;
using Eclipse.Infrastructure.Builder;
using Eclipse.Infrastructure.Telegram;
using Eclipse.Pipelines;
using Eclipse.WebAPI;
using Eclipse.WebAPI.Filters;
using Eclipse.WebAPI.Middlewares;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services
    .AddDomainSharedModule()
    .AddDomainModule()
    .AddCoreModule()
    .AddApplicationModule()
    .AddApplicationContractsModule()
    .AddPipelinesModule()
    .AddWebApiModule()
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
    });

builder.Services.Configure<ApiKeyAuthorizationOptions>(
    builder.Configuration.GetSection("Authorization")
);

builder.Host.UseSerilog((_, config) =>
{
    config.WriteTo.Console();
});

var app = builder.Build();

await app.Services.GetRequiredService<IEclipseStarter>()
    .StartAsync();

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
