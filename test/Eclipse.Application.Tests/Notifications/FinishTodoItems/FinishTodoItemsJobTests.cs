using Eclipse.Application.Jobs;
using Eclipse.Application.TodoItems.Finish;
using Eclipse.Domain.Users;
using Eclipse.Localization.Culture;
using Eclipse.Tests.Extensions;
using Eclipse.Tests.Generators;

using FluentAssertions;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using Quartz;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Requests;

using Xunit;

namespace Eclipse.Application.Tests.Notifications.FinishTodoItems;

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
            { "data", JsonConvert.SerializeObject(new UserIdJobData(Guid.NewGuid())) }
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
            { "data", JsonConvert.SerializeObject(new UserIdJobData(user.Id)) }
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
            { "data", JsonConvert.SerializeObject(new UserIdJobData(user.Id)) }
        };

        context.MergedJobDataMap.Returns(map);

        await _sut.Execute(context);

        await _client.Received().SendRequest(
            Arg.Is<SendMessageRequest>(r => r.ChatId == user.ChatId && r.Text == "You have 2 pending tasks")
        );
    }

    [Fact]
    public async Task Execute_WhenClientThrowsException_ThenLogsExceptionEndDisablesUser()
    {
        var user = UserGenerator.Get();

        _repository.FindAsync(user.Id).Returns(user);

        _localizer["Jobs:Evening:RemindMarkAsFinished", user.Name, user.TodoItems.Count]
            .Returns(new LocalizedString("", "You have 2 pending tasks"));

        var context = Substitute.For<IJobExecutionContext>();

        var map = new JobDataMap
        {
            { "data", JsonConvert.SerializeObject(new UserIdJobData(user.Id)) }
        };

        context.MergedJobDataMap.Returns(map);

        _client.SendRequest(Arg.Any<SendMessageRequest>())
            .ThrowsAsync(new ApiRequestException("test"));

        await _sut.Execute(context);

        _logger.ShouldReceiveLog(LogLevel.Error);
        user.IsEnabled.Should().BeFalse();
        await _repository.Received().UpdateAsync(user);
    }
}
