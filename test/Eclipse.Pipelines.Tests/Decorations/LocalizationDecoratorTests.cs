using Eclipse.Core.Core;
using Eclipse.Core.Models;
using Eclipse.Core.Results;
using Eclipse.Domain.Users;
using Eclipse.Localization.Culture;
using Eclipse.Pipelines.Culture;
using Eclipse.Pipelines.Decorations;
using Eclipse.Tests.Generators;

using NSubstitute;
using NSubstitute.ReturnsExtensions;

using System.Globalization;

using Xunit;

namespace Eclipse.Pipelines.Tests.Decorations;

public sealed class LocalizationDecoratorTests
{
    private readonly IUserRepository _repository;

    private readonly ICultureTracker _cultureTracker;

    private readonly ICurrentCulture _currentCulture;

    private readonly Func<MessageContext, CancellationToken, Task<IResult>> _execution;

    private readonly LocalizationDecorator _sut;

    public LocalizationDecoratorTests()
    {
        _repository = Substitute.For<IUserRepository>();
        _cultureTracker = Substitute.For<ICultureTracker>();
        _currentCulture = Substitute.For<ICurrentCulture>();

        _execution = (_, _) => Task.FromResult<IResult>(new EmptyResult());

        _sut = new LocalizationDecorator(_repository, _cultureTracker, _currentCulture);
    }

    [Fact]
    public async Task Decorate_WhenLocalizationNotSpecified_ThenFetchingUserCulture()
    {
        var user = UserGenerator.Get();
        var services = Substitute.For<IServiceProvider>();

        var context = new MessageContext(user.ChatId, string.Empty, new TelegramUser(), services);

        _cultureTracker.GetAsync(user.ChatId).ReturnsNull();

        _repository.FindByChatIdAsync(user.ChatId).Returns(user);

        await _sut.Decorate(_execution, context);

        _currentCulture.Received().UsingCulture(
            Arg.Is<CultureInfo>(x => x.Name == user.Culture)
        );

        await _repository.Received().FindByChatIdAsync(user.ChatId);

        await _cultureTracker.Received().GetAsync(user.ChatId);
        await _cultureTracker.Received().ResetAsync(user.ChatId, user.Culture);
    }
}
