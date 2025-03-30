using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Localization.Culture;
using Eclipse.Pipelines.Culture;
using Eclipse.Pipelines.Localization;

using NSubstitute;
using NSubstitute.ReturnsExtensions;

using System.Globalization;

using Xunit;

namespace Eclipse.Pipelines.Tests.Decorations;

public sealed class LocalizationDecoratorTests
{
    private readonly ICurrentCulture _currentCulture;

    private readonly ICultureTracker _cultureTracker;

    private readonly Func<MessageContext, CancellationToken, Task<IResult>> _execution;

    private readonly LocalizationDecorator _sut;

    public LocalizationDecoratorTests()
    {
        _currentCulture = Substitute.For<ICurrentCulture>();
        _cultureTracker = Substitute.For<ICultureTracker>();

        _execution = (_, _) => Task.FromResult<IResult>(new EmptyResult());

        _sut = new LocalizationDecorator(_currentCulture, _cultureTracker);
    }

    [Theory]
    [InlineData(1)]
    public async Task Decorate_WhenCultureNotSpecified_ThenUsesDefaultCulture(long chatId)
    {
        var context = new MessageContext(chatId, string.Empty, new TelegramUser(), Substitute.For<IServiceProvider>());

        _cultureTracker.GetAsync(chatId).ReturnsNull();

        await _sut.Decorate(_execution, context);

        _currentCulture.Received().UsingCulture(null);

        await _cultureTracker.Received().GetAsync(chatId);
    }

    [Theory]
    [InlineData(1, "en")]
    public async Task Decorate_WhenCultureSpecified_ThenUsesIt(long chatId, string culture)
    {
        var context = new MessageContext(chatId, string.Empty, new TelegramUser(), Substitute.For<IServiceProvider>());

        _cultureTracker.GetAsync(chatId).Returns(culture);

        await _sut.Decorate(_execution, context);

        _currentCulture.Received().UsingCulture(
            Arg.Is<CultureInfo>(c => c.Name == culture)
        );

        await _cultureTracker.Received().GetAsync(chatId);
    }
}
