using Eclipse.Application;
using Eclipse.Infrastructure;
using Eclipse.Infrastructure.Builder;
using Eclipse.Infrastructure.Telegram;
using Eclipse.WebAPI.Filters;
using Eclipse.WebAPI.Helpers;
using Eclipse.WebAPI.Middlewares;
using Eclipse.WebAPI.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInfrastructure(config =>
    {
        config.TelegramOptions = new TelegramOptions
        {
            Token = builder.Configuration["Telegram:Token"]!,
        };

        config.UseTelegramHandler<TelegramUpdateHandler>();

        config.CacheOptions = new CacheOptions
        {
            Expiration = new TimeSpan(1, 0, 0, 0)
        };
    });

builder.Services
    .AddApplication()
    .AddCore();

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
