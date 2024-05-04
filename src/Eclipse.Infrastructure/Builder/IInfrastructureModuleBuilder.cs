using Eclipse.Common.Telegram;

using Telegram.Bot.Polling;

namespace Eclipse.Infrastructure.Builder;

public interface IInfrastructureModuleBuilder
{
    IInfrastructureModuleBuilder UseTelegramHandler<THandler>()
        where THandler : IUpdateHandler;

    IInfrastructureModuleBuilder ConfigureTelegramOptions(Action<TelegramOptions> options);

    IInfrastructureModuleBuilder ConfigureGoogleOptions(Action<GoogleOptions> options);

    IInfrastructureModuleBuilder ConfigureCacheOptions(Action<CacheOptions> options);
}
