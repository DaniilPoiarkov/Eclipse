using Eclipse.Application.Account;
using Eclipse.Application.Account.Background;
using Eclipse.Common.Background;
using Eclipse.Common.Clock;
using Eclipse.Domain.Shared.Users;
using Eclipse.Domain.Users;
using Eclipse.Tests.Generators;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Account;

public sealed class AccountServiceTests
{
    private readonly IUserRepository _userRepository;

    private readonly ITimeProvider _timeProvider;

    private readonly IBackgroundJobManager _backgroundJobManager;

    private readonly AccountService _sut;

    public AccountServiceTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _timeProvider = Substitute.For<ITimeProvider>();
        _backgroundJobManager = Substitute.For<IBackgroundJobManager>();
        _sut = new AccountService(new UserManager(_userRepository), _timeProvider, _backgroundJobManager);
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

        var result = await _sut.SendSignInCodeAsync(user.UserName);

        result.IsSuccess.Should().BeTrue();

        user.SignInCode.Should().NotBe(expiredSignInCode);
        user.SignInCodeExpiresAt.Should().Be(now.Add(UserConsts.SignInCodeExpiration));

        await _userRepository.Received(1).UpdateAsync(user);

        await _backgroundJobManager.Received(1).EnqueueAsync<SendSignInCodeBackgroundJob, SendSignInCodeArgs>(
            Arg.Is<SendSignInCodeArgs>(x => x.ChatId == user.ChatId
                && x.Culture == user.Culture
                && x.SignInCode == user.SignInCode)
        );
    }
}
