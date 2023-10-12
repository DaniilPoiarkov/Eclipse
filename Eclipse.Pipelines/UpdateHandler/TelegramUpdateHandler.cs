using Microsoft.Extensions.DependencyInjection;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Pipelines.UpdateHandler;

internal class TelegramUpdateHandler : ITelegramUpdateHandler
{
    private readonly IServiceProvider _serviceProvider;

    public TelegramUpdateHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IEclipseUpdateHandler>();

        await handler.HandlePollingErrorAsync(botClient, exception, cancellationToken);
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IEclipseUpdateHandler>();

        try
        {
            await handler.HandleUpdateAsync(botClient, update, cancellationToken);
        }
        catch (Exception ex)
        {
            await handler.HandlePollingErrorAsync(botClient, ex, cancellationToken);
        }
    }
}
