using Eclipse.Application.Account.Background;
using Eclipse.Application.Contracts.Telegram;
using Eclipse.Common.Results;
using Eclipse.Localization.Culture;

using Microsoft.Extensions.Localization;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Account;

public sealed class SendSignInCodeBackgroundJobTests
{
    private readonly IStringLocalizer<SendSignInCodeBackgroundJob> _stringLocalizer;

    private readonly ITelegramService _telegramService;

    private readonly ICurrentCulture _currentCulture;

    private readonly SendSignInCodeBackgroundJob _sut;

    public SendSignInCodeBackgroundJobTests()
    {
        _stringLocalizer = Substitute.For<IStringLocalizer<SendSignInCodeBackgroundJob>>();
        _telegramService = Substitute.For<ITelegramService>();
        _currentCulture = Substitute.For<ICurrentCulture>();

        _sut = new(_stringLocalizer, _telegramService, _currentCulture);
    }

    [Theory]
    [InlineData("en", 1, "123456")]
    [InlineData("en", 2, "123456")]
    [InlineData("uk", 1, "123456")]
    [InlineData("uk", 2, "123456")]
    public async Task ExecuteAsync_WhenCalledProperly_ThenSendsSignInCode(string culture, long chatId, string signInCode)
    {
        var args = new SendSignInCodeArgs
        {
            Culture = culture,
            ChatId = chatId,
            SignInCode = signInCode
        };

        _telegramService.Send(
                Arg.Is<SendMessageModel>(x => x.ChatId == chatId)
            )
            .Returns(
                Task.FromResult(Result.Success())
            );

        await _sut.ExecuteAsync(args);

        using var _ = _currentCulture.Received().UsingCulture(args.Culture);

        var message = _stringLocalizer.Received()["Account:{0}AuthenticationCode", args.SignInCode];

        await _telegramService.Received(1)
            .Send(
                Arg.Is<SendMessageModel>(x => x.ChatId == args.ChatId && x.Message == message)
            );
    }
}
