using Eclipse.Application.Contracts.Localizations;
using Eclipse.Core.Core;
using Eclipse.Core.Models;
using Eclipse.Core.Results;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Infrastructure.Cache;
using Eclipse.Pipelines.Decorations;
using Eclipse.Tests.Generators;

using Microsoft.Extensions.DependencyInjection;

using NSubstitute;
using NSubstitute.ReturnsExtensions;

using Xunit;

namespace Eclipse.Pipelines.Tests.Decorations;

public class LocalizationDecoratorTests
{
    private readonly IdentityUserManager _userManager;

    private readonly ICacheService _cacheService;

    private readonly IEclipseLocalizer _localizer;

    private readonly Lazy<LocalizationDecorator> _lazySut;

    private readonly Func<MessageContext, CancellationToken, Task<IResult>> _execution;

    private LocalizationDecorator Sut => _lazySut.Value;

    public LocalizationDecoratorTests()
    {
        _userManager = Substitute.For<IdentityUserManager>(Substitute.For<IIdentityUserRepository>());
        _cacheService = Substitute.For<ICacheService>();
        _localizer = Substitute.For<IEclipseLocalizer>();

        _execution = (_, _) => Task.FromResult<IResult>(new EmptyResult());

        _lazySut = new Lazy<LocalizationDecorator>(() => new LocalizationDecorator(_userManager, _cacheService, _localizer));
    }

    [Fact]
    public async Task Decorate_WhenLocalizationNotSpecified_ThenFetchingUserCulture()
    {
        var user = IdentityUserGenerator.Generate(1).First();
        var services = new ServiceCollection().BuildServiceProvider();
        
        var context = new MessageContext(user.ChatId, string.Empty, new TelegramUser(), services);

        _cacheService.Get<string>(default!).ReturnsNullForAnyArgs();
        _userManager.FindByChatIdAsync(user.ChatId).Returns(Task.FromResult<IdentityUser?>(user));

        await Sut.Decorate(_execution, context);

        _cacheService.ReceivedWithAnyArgs().Get<string>(default!);
        await _userManager.Received().FindByChatIdAsync(user.ChatId);
        _cacheService.ReceivedWithAnyArgs().Set(default!, user.Culture);
        _localizer.Received().CheckCulture(user.ChatId);
    }
}
