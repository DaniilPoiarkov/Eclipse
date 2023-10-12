using Eclipse.Application.Contracts.Localizations;
using Eclipse.Core.Core;
using Eclipse.Pipelines.CachedServices;
using Eclipse.Pipelines.Pipelines;

using Microsoft.Extensions.DependencyInjection;

using NSubstitute;

using Telegram.Bot;

namespace Eclipse.Pipelines.Tests.Fixture;

public abstract class PipelineTestFixture<TPipeline>
    where TPipeline : EclipsePipelineBase
{
    protected readonly ITelegramBotClient BotClient;

    protected readonly ICurrentTelegramUser CurrentTelegramUser;

    protected readonly IEclipseLocalizer Localizer;

    protected readonly IServiceProvider ServiceProvider;

    protected readonly TPipeline Sut;

    private readonly Func<TPipeline> _factory;

    public PipelineTestFixture(Func<TPipeline> factory)
    {
        _factory = factory;

        BotClient = Substitute.For<ITelegramBotClient>();
        CurrentTelegramUser = Substitute.For<ICurrentTelegramUser>();
        Localizer = Substitute.For<IEclipseLocalizer>();

        var services = new ServiceCollection()
            .AddSingleton(_factory());

        ConfigureServices(services);

        ServiceProvider = services.BuildServiceProvider();

        CachedServiceProvider.SetServiceProvider(ServiceProvider);

        Sut = ServiceProvider.GetRequiredService<TPipeline>();
    }

    protected virtual void ConfigureServices(IServiceCollection services)
    {
        services
            .AddSingleton(CurrentTelegramUser)
            .AddSingleton(Localizer)
            .AddSingleton(BotClient);
    }

    protected void AssertResult<TResult>(TResult result, Action<ResultAssertion<TResult>> assertion)
        where TResult : IResult
    {
        var resultAssertion = new ResultAssertion<TResult>(result);
        assertion(resultAssertion);
    }
}
