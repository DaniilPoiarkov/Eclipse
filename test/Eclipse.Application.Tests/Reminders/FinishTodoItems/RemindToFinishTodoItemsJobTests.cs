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

public sealed class RemindToFinishTodoItemsJobTests
{
    private readonly IStringLocalizer<RemindToFinishTodoItemsJob> _localizer;

    private readonly ICurrentCulture _currentCulture;

    private readonly ITelegramBotClient _client;

    private readonly IUserRepository _repository;

    private readonly ILogger<RemindToFinishTodoItemsJob> _logger;

    private readonly RemindToFinishTodoItemsJob _sut;

    public RemindToFinishTodoItemsJobTests()
    {
        _localizer = Substitute.For<IStringLocalizer<RemindToFinishTodoItemsJob>>();
        _currentCulture = Substitute.For<ICurrentCulture>();
        _client = Substitute.For<ITelegramBotClient>();
        _repository = Substitute.For<IUserRepository>();
        _logger = Substitute.For<ILogger<RemindToFinishTodoItemsJob>>();

        _sut = new RemindToFinishTodoItemsJob(_localizer, _currentCulture, _client, _repository, _logger);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("{\"InvalidJson\": \"\"}")]
    public async Task Execute_WhenDataInvalid_ThenLogsError(string? data)
    {
        var context = Substitute.For<IJobExecutionContext>();

        var dataMap = new JobDataMap
        {
            { "data", data! }
        };

        context.MergedJobDataMap.Returns(dataMap);

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
    public async Task Execute_WhenUserNotFound_ThenLogsError()
    {
        var context = Substitute.For<IJobExecutionContext>();
        var data = JsonConvert.SerializeObject(new RemindToFinishTodoItemsJobData(Guid.NewGuid()));

        var dataMap = new JobDataMap
        {
            { "data", data }
        };

        context.MergedJobDataMap.Returns(dataMap);

        _repository.FindAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(null));

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
        var context = Substitute.For<IJobExecutionContext>();
        var userId = Guid.NewGuid();
        var data = JsonConvert.SerializeObject(new RemindToFinishTodoItemsJobData(userId));
        var user = UserGenerator.Get();

        var dataMap = new JobDataMap
        {
            { "data", data }
        };

        context.MergedJobDataMap.Returns(dataMap);

        _repository.FindAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

        _localizer["Jobs:Evening:Empty", user.Name, 0]
            .Returns(new LocalizedString("", "No pending tasks"));

        await _sut.Execute(context);

        await _client.Received().SendRequest(
            Arg.Is<SendMessageRequest>(r => r.ChatId == user.ChatId && r.Text == "No pending tasks")
        );
    }

    [Fact]
    public async Task Execute_WhenTodoItemsArePresent_ThenSendsReminderMessage()
    {
        var context = Substitute.For<IJobExecutionContext>();
        var userId = Guid.NewGuid();
        var data = JsonConvert.SerializeObject(new RemindToFinishTodoItemsJobData(userId));
        var user = UserGenerator.Get();

        user.AddTodoItem("test", DateTime.UtcNow);
        user.AddTodoItem("test", DateTime.UtcNow);

        var dataMap = new JobDataMap
        {
            { "data", data }
        };

        context.MergedJobDataMap.Returns(dataMap);
        _repository.FindAsync(userId).Returns(user);

        _localizer["Jobs:Evening:RemindMarkAsFinished", user.Name, user.TodoItems.Count]
            .Returns(new LocalizedString("", "You have 2 pending tasks"));

        await _sut.Execute(context);

        await _client.Received().SendRequest(
            Arg.Is<SendMessageRequest>(r => r.ChatId == user.ChatId && r.Text == "You have 2 pending tasks")
        );
    }
}
