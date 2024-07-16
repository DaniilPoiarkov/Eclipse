using Eclipse.Core.Core;
using Eclipse.Pipelines.Pipelines;
using Eclipse.Tests.Generators;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using NSubstitute;

using Telegram.Bot;

namespace Eclipse.Pipelines.Tests.Fixture;

public abstract class PipelineTestFixture<TPipeline>
    where TPipeline : EclipsePipelineBase
{
    protected readonly ITelegramBotClient BotClient;

    protected readonly ICurrentTelegramUser CurrentTelegramUser;

    protected readonly IStringLocalizer Localizer;

    protected readonly IServiceProvider ServiceProvider;

    protected readonly TPipeline Sut;

    public PipelineTestFixture()
    {
        BotClient = Substitute.For<ITelegramBotClient>();
        CurrentTelegramUser = Substitute.For<ICurrentTelegramUser>();
        Localizer = Substitute.For<IStringLocalizer>();

        var services = new ServiceCollection()
            .AddSingleton<TPipeline>()
            .AddSingleton(CurrentTelegramUser)
            .AddSingleton(Localizer)
            .AddSingleton(BotClient);

        ConfigureServices(services);

        ServiceProvider = services.BuildServiceProvider();
        Sut = ServiceProvider.GetRequiredService<TPipeline>();
        Sut.SetLocalizer(Localizer);
    }

    protected virtual void ConfigureServices(IServiceCollection services)
    {

    }

    protected MessageContext GetContext(string value)
    {
        return MessageContextGenerator.Generate(value, ServiceProvider);
    }
}
