using Eclipse.Application.Caching;
using Eclipse.Common.Cache;
using Eclipse.Core.Core;
using Eclipse.Core.Models;
using Eclipse.Core.Results;
using Eclipse.Domain.Users;
using Eclipse.Localization.Culture;
using Eclipse.Pipelines.Decorations;
using Eclipse.Tests.Generators;

using Microsoft.Extensions.DependencyInjection;

using NSubstitute;
using NSubstitute.ReturnsExtensions;

using Xunit;

namespace Eclipse.Pipelines.Tests.Decorations;

public sealed class LocalizationDecoratorTests
{
    private readonly IUserRepository _repository;

    private readonly ICacheService _cacheService;

    private readonly ICurrentCulture _currentCulture;

    private readonly Lazy<LocalizationDecorator> _lazySut;

    private readonly Func<MessageContext, CancellationToken, Task<IResult>> _execution;

    private LocalizationDecorator Sut => _lazySut.Value;

    public LocalizationDecoratorTests()
    {
        _repository = Substitute.For<IUserRepository>();
        _cacheService = Substitute.For<ICacheService>();
        _currentCulture = Substitute.For<ICurrentCulture>();

        _execution = (_, _) => Task.FromResult<IResult>(new EmptyResult());

        _lazySut = new Lazy<LocalizationDecorator>(
            () => new LocalizationDecorator(
                new UserManager(_repository),
                _cacheService,
                _currentCulture));
    }

    [Fact]
    public async Task Decorate_WhenLocalizationNotSpecified_ThenFetchingUserCulture()
    {
        var user = UserGenerator.Generate(1).First();
        var services = new ServiceCollection().BuildServiceProvider();

        var context = new MessageContext(user.ChatId, string.Empty, new TelegramUser(), services);

        _cacheService.GetAsync<string>(default!).ReturnsNullForAnyArgs();

        _repository.GetByExpressionAsync(_ => true)
            .ReturnsForAnyArgs(Task.FromResult<IReadOnlyList<User>>([user]));

        await Sut.Decorate(_execution, context);

        _currentCulture.Received().UsingCulture(user.Culture);
        await _cacheService.ReceivedWithAnyArgs().GetAsync<string>(default!);
        await _repository.ReceivedWithAnyArgs().GetByExpressionAsync(_ => true);
        await _cacheService.ReceivedWithAnyArgs().SetAsync(default!, user.Culture, CacheConsts.ThreeDays);
    }
}
