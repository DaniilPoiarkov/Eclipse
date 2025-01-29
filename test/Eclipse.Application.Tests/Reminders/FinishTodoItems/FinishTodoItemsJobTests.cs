using Eclipse.Application.Reminders.FinishTodoItems;
using Eclipse.Domain.Users;
using Eclipse.Localization.Culture;
using Eclipse.Tests.Generators;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using NSubstitute;

using Quartz;

using Telegram.Bot;
using Telegram.Bot.Requests;

using Xunit;

namespace Eclipse.Application.Tests.Reminders.FinishTodoItems;

public sealed class FinishTodoItemsJobTests
{
    private readonly IStringLocalizer<FinishTodoItemsJob> _localizer;

    private readonly ICurrentCulture _currentCulture;

    private readonly ITelegramBotClient _client;

    private readonly IUserRepository _repository;

    private readonly ILogger<FinishTodoItemsJob> _logger;

    private readonly FinishTodoItemsJob _sut;

    public FinishTodoItemsJobTests()
    {
        _localizer = Substitute.For<IStringLocalizer<FinishTodoItemsJob>>();
        _currentCulture = Substitute.For<ICurrentCulture>();
        _client = Substitute.For<ITelegramBotClient>();
        _repository = Substitute.For<IUserRepository>();
        _logger = Substitute.For<ILogger<FinishTodoItemsJob>>();

        _sut = new FinishTodoItemsJob(_localizer, _currentCulture, _client, _repository, _logger);
    }

    [Fact]
    public async Task Execute_WhenUserNotFound_ThenLogsError()
    {
        _repository.FindAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(null));

        var context = Substitute.For<IJobExecutionContext>();

        var map = new JobDataMap
        {
            { "data", JsonConvert.SerializeObject(new FinishTodoItemsJobData(Guid.NewGuid())) }
        };

        context.MergedJobDataMap.Returns(map);

        await _sut.Execute(context);

        _logger.Received().Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>()
        );
    }

    [Fact]
    public async Task Execute_WhenTodoItemsAreEmpty_ThenSendsEmptyReminderMessage()
    {
        var user = UserGenerator.Get();

        _repository.FindAsync(user.Id).Returns(user);

        _localizer["Jobs:Evening:Empty", user.Name, 0]
            .Returns(new LocalizedString("", "No pending tasks"));

        var context = Substitute.For<IJobExecutionContext>();

        var map = new JobDataMap
        {
            { "data", JsonConvert.SerializeObject(new FinishTodoItemsJobData(user.Id)) }
        };

        context.MergedJobDataMap.Returns(map);

        await _sut.Execute(context);

        await _client.Received().SendRequest(
            Arg.Is<SendMessageRequest>(r => r.ChatId == user.ChatId && r.Text == "No pending tasks")
        );
    }

    [Fact]
    public async Task Execute_WhenTodoItemsArePresent_ThenSendsReminderMessage()
    {
        var user = UserGenerator.Get();

        user.AddTodoItem("test", DateTime.UtcNow);
        user.AddTodoItem("test", DateTime.UtcNow);

        _repository.FindAsync(user.Id).Returns(user);

        _localizer["Jobs:Evening:RemindMarkAsFinished", user.Name, user.TodoItems.Count]
            .Returns(new LocalizedString("", "You have 2 pending tasks"));

        var context = Substitute.For<IJobExecutionContext>();

        var map = new JobDataMap
        {
            { "data", JsonConvert.SerializeObject(new FinishTodoItemsJobData(user.Id)) }
        };

        context.MergedJobDataMap.Returns(map);

        await _sut.Execute(context);

        await _client.Received().SendRequest(
            Arg.Is<SendMessageRequest>(r => r.ChatId == user.ChatId && r.Text == "You have 2 pending tasks")
        );
    }
}
