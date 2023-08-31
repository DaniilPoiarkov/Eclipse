using Eclipse.WebAPI;
using Eclipse.WebAPI.Middlewares;
using Eclipse.WebAPI.TelegramHandler;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Telegram.Bot;
using Telegram.Bot.Polling;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.Scan(tss => 
    tss.FromAssemblyOf<WeatherForecast>()
        .AddClasses()
        .AsMatchingInterface()
        .WithTransientLifetime());

builder.Services.AddTransient<IUpdateHandler, TelegramUpdateHandler>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var telegramConfig = builder.Configuration.GetSection("Telegram");

var client = new TelegramBotClient(telegramConfig["Token"]!);
builder.Services.AddSingleton<ITelegramBotClient>(client);

var app = builder.Build();

var eclipse = await client.GetMeAsync();

app.Logger.LogInformation("Bot: {token}", eclipse.Username);
client.StartReceiving(app.Services.GetRequiredService<IUpdateHandler>());

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    ///
}

app.UseMiddleware<GlobalExceptionHandler>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
