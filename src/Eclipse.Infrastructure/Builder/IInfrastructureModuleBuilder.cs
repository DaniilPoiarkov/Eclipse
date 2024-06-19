using Eclipse.Common.Telegram;

namespace Eclipse.Infrastructure.Builder;

public interface IInfrastructureModuleBuilder
{
    IInfrastructureModuleBuilder ConfigureTelegramOptions(Action<TelegramOptions> options);

    IInfrastructureModuleBuilder ConfigureGoogleOptions(Action<GoogleOptions> options);
}
