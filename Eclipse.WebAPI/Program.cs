using Eclipse.WebAPI.Helpers;
using Eclipse.WebAPI.Middlewares;
using Eclipse.WebAPI.Services.TelegramServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure();

var client = builder.AddEclipseBot();

var app = builder.Build();

await app.Services.GetRequiredService<IEclipseStarter>()
    .StartAsync();

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    ///
}

app.UseMiddleware<GlobalExceptionHandler>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
