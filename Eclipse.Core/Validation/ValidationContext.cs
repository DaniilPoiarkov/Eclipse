using Eclipse.Core.Models;

namespace Eclipse.Core.Validation;

public sealed class ValidationContext
{
    public IServiceProvider ServiceProvider { get; }

    public TelegramUser? TelegramUser { get; }

    public ValidationContext(IServiceProvider serviceProvider, TelegramUser? telegramUser)
    {
        ServiceProvider = serviceProvider;
        TelegramUser = telegramUser;
    }
}
