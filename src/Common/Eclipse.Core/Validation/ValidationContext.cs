using Telegram.Bot.Types;

namespace Eclipse.Core.Validation;

public sealed class ValidationContext
{
    public IServiceProvider ServiceProvider { get; }

    public Update Update { get; }

    public ValidationContext(IServiceProvider serviceProvider, Update update)
    {
        ServiceProvider = serviceProvider;
        Update = update;
    }
}
