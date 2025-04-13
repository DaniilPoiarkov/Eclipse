using Eclipse.Core.Context;
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

    protected readonly IStringLocalizer Localizer;

    protected readonly IServiceProvider ServiceProvider;

    protected readonly TPipeline Sut;

    public PipelineTestFixture()
    {
        BotClient = Substitute.For<ITelegramBotClient>();
        Localizer = Substitute.For<IStringLocalizer>();

        var services = new ServiceCollection()
            .AddSingleton<TPipeline>()
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

    protected TService GetService<TService>()
        where TService : notnull
    {
        return ServiceProvider.GetRequiredService<TService>();
    }
}
