using Eclipse.Application.Account;
using Eclipse.Application.Contracts.Telegram;
using Eclipse.Common.Clock;
using Eclipse.Common.Results;
using Eclipse.Domain.Shared.Users;
using Eclipse.Domain.Users;
using Eclipse.Localization.Culture;
using Eclipse.Localization.Extensions;
using Eclipse.Tests.Generators;

using FluentAssertions;

using Microsoft.Extensions.Localization;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Account;

public sealed class AccountServiceTests
{
    private readonly IUserRepository _userRepository;

    private readonly ITelegramService _telegramService;

    private readonly IStringLocalizer<AccountService> _stringLocalizer;

    private readonly ICurrentCulture _currentCulture;

    private readonly ITimeProvider _timeProvider;

    private readonly AccountService _sut;

    public AccountServiceTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _telegramService = Substitute.For<ITelegramService>();
        _stringLocalizer = Substitute.For<IStringLocalizer<AccountService>>();
        _currentCulture = Substitute.For<ICurrentCulture>();
        _timeProvider = Substitute.For<ITimeProvider>();
        _sut = new AccountService(new UserManager(_userRepository), _telegramService, _stringLocalizer, _currentCulture, _timeProvider);
    }

    [Fact]
    public async Task SendSignInCodeAsync_WhenRequested_THenSendsSignInCode()
    {
        var past = new DateTime(new DateOnly(1990, 1, 1), new TimeOnly(12, 0));
        var user = UserGenerator.Get();

        user.SetSignInCode(past);

        var expiredSignInCode = user.SignInCode;

        _userRepository.GetByExpressionAsync(_ => true)
            .ReturnsForAnyArgs(
                Task.FromResult<IReadOnlyList<User>>([user])
            );

        var now = new DateTime(new DateOnly(1990, 1, 1), new TimeOnly(15, 0));

        _timeProvider.Now.Returns(now);

        _telegramService.Send(default!)
            .ReturnsForAnyArgs(
                Task.FromResult(Result.Success())
            );

        var result = await _sut.SendSignInCodeAsync(user.UserName);

        result.IsSuccess.Should().BeTrue();

        user.SignInCode.Should().NotBe(expiredSignInCode);
        user.SignInCodeExpiresAt.Should().Be(now.Add(UserConsts.SignInCodeExpiration));

        using var _ = _currentCulture.Received(1).UsingCulture(user.Culture);
        _stringLocalizer.Received(1).UseCurrentCulture(_currentCulture);

        await _userRepository.Received(1).UpdateAsync(user);

        var message = _stringLocalizer.Received(1)["Account:{0}AuthenticationCode", user.SignInCode];

        await _telegramService.Received(1)
            .Send(
                Arg.Is<SendMessageModel>(x => x.ChatId == user.ChatId && x.Message == message)
            );
    }
}
