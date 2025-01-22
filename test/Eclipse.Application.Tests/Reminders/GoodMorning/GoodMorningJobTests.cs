using Eclipse.Application.Reminders.GoodMorning;
using Eclipse.Domain.Users;
using Eclipse.Localization.Culture;
using Eclipse.Tests.Generators;

using Microsoft.Extensions.Localization;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Telegram.Bot;
using Telegram.Bot.Requests;

using Xunit;

namespace Eclipse.Application.Tests.Reminders.GoodMorning;

public sealed class GoodMorningJobTests
{
    private readonly ICurrentCulture _currentCulture;

    private readonly IStringLocalizer<GoodMorningJob> _localizer;

    private readonly ITelegramBotClient _client;

    private readonly IUserRepository _repository;

    private readonly ILogger<GoodMorningJob> _logger;

    private readonly GoodMorningJob _sut;

    public GoodMorningJobTests()
    {
        _currentCulture = Substitute.For<ICurrentCulture>();
        _localizer = Substitute.For<IStringLocalizer<GoodMorningJob>>();
        _client = Substitute.For<ITelegramBotClient>();
        _repository = Substitute.For<IUserRepository>();
        _logger = Substitute.For<ILogger<GoodMorningJob>>();

        _sut = new GoodMorningJob(_currentCulture, _localizer, _client, _repository, _logger);
    }

    [Fact]
    public async Task Execute_WhenUserNotFound_ThenLogsError()
    {
        _repository.FindAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(null));

        var args = new GoodMorningJobData(Guid.NewGuid());

        await _sut.Handle(args);

        _logger.Received().Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>()
        );
    }

    [Fact]
    public async Task Execute_WhenUserFound_ThenSendsNotification()
    {
        var user = UserGenerator.Get();

        _repository.FindAsync(user.Id).Returns(user);

        _localizer[Arg.Any<string>()].Returns(new LocalizedString("", "good morning"));

        var args = new GoodMorningJobData(user.Id);

        await _sut.Handle(args);

        _currentCulture.Received().UsingCulture(user.Culture);

        await _client.Received().SendRequest(
            Arg.Is<SendMessageRequest>(r => r.ChatId == user.ChatId && r.Text == "good morning")
        );
    }
}
