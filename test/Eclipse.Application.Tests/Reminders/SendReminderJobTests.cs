using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.Reminders.Sendings;
using Eclipse.Common.Results;
using Eclipse.Domain.Reminders;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Users.Events;
using Eclipse.Localization.Culture;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using NSubstitute;

using Quartz;

using System.Globalization;

using Telegram.Bot;
using Telegram.Bot.Requests;

using Xunit;

namespace Eclipse.Application.Tests.Reminders;

// TODO: FIX TESTS
public sealed class SendReminderJobTests
{
    //private readonly ITelegramBotClient _client;

    //private readonly IStringLocalizer<SendReminderJob> _localizer;

    //private readonly IReminderService _reminderService;

    //private readonly ILogger<SendReminderJob> _logger;

    //private readonly ICurrentCulture _currentCulture;

    //private readonly SendReminderJob _sut;

    //public SendReminderJobTests()
    //{
    //    _client = Substitute.For<ITelegramBotClient>();
    //    _localizer = Substitute.For<IStringLocalizer<SendReminderJob>>();
    //    _reminderService = Substitute.For<IReminderService>();
    //    _logger = Substitute.For<ILogger<SendReminderJob>>();
    //    _currentCulture = Substitute.For<ICurrentCulture>();

    //    _sut = new SendReminderJob(_client, _localizer, _reminderService, _logger, _currentCulture);
    //}

    //[Theory]
    //[InlineData("test", "en", 1)]
    //public async Task Execute_WhenTriggered_ThenSendsReminder_AndUnschedulesJob(string text, string culture, long chatId)
    //{
    //    var userId = Guid.NewGuid();
    //    var reminderId = Guid.NewGuid();

    //    var data = new ReminderAddedDomainEvent(reminderId, userId, new TimeSpan(), new TimeOnly(), text, culture, chatId);

    //    var map = new JobDataMap((IDictionary<string, object>)new Dictionary<string, object>
    //    {
    //        ["data"] = JsonConvert.SerializeObject(data)
    //    });

    //    var context = Substitute.For<IJobExecutionContext>();

    //    context.MergedJobDataMap.Returns(map);

    //    var scheduler = Substitute.For<IScheduler>();
    //    context.Scheduler.Returns(scheduler);

    //    _localizer["Jobs:SendReminders:Message"]
    //        .Returns(new LocalizedString("Jobs:SendReminders:Message", "Reminder:"));

    //    _reminderService.DeleteAsync(userId, reminderId).Returns(Result.Success());

    //    await _sut.Execute(context);

    //    _currentCulture.Received().UsingCulture(Arg.Is<CultureInfo>(c => c.Name == culture));

    //    await _client.Received().SendRequest(
    //        Arg.Is<SendMessageRequest>(request => request.ChatId == chatId
    //            && request.Text == $"Reminder:\n\r\n\r{text}"
    //        )
    //    );

    //    await _reminderService.Received().DeleteAsync(userId, reminderId);
    //    await scheduler.Received().UnscheduleJob(Arg.Any<TriggerKey>());
    //}

    //[Theory]
    //[InlineData("")]
    //[InlineData(null)]
    //[InlineData("\"\"")]
    //public async Task Execute_WhenDataInvalid_ThenLogsError(string? data)
    //{
    //    var context = Substitute.For<IJobExecutionContext>();

    //    var map = new JobDataMap((IDictionary<string, object>)new Dictionary<string, object>
    //    {
    //        ["data"] = data!
    //    });

    //    context.MergedJobDataMap.Returns(map);

    //    await _sut.Execute(context);

    //    _logger.Received().Log(
    //        LogLevel.Error,
    //        Arg.Any<EventId>(),
    //        Arg.Any<object>(),
    //        Arg.Any<Exception>(),
    //        Arg.Any<Func<object, Exception?, string>>()
    //    );

    //    _currentCulture.DidNotReceive().UsingCulture(Arg.Any<CultureInfo>());
    //    await _client.DidNotReceive().SendRequest(Arg.Any<SendMessageRequest>());
    //    await _reminderService.DidNotReceive().DeleteAsync(Arg.Any<Guid>(), Arg.Any<Guid>());
    //}

    //[Theory]
    //[InlineData("text", "en", 1)]
    //public async Task Execute_WhenDeletionFails_ThenLoggsError(string text, string culture, long chatId)
    //{
    //    var userId = Guid.NewGuid();
    //    var reminderId = Guid.NewGuid();

    //    var data = new ReminderAddedDomainEvent(reminderId, userId, new TimeSpan(), new TimeOnly(), text, culture, chatId);

    //    var map = new JobDataMap((IDictionary<string, object>)new Dictionary<string, object>
    //    {
    //        ["data"] = JsonConvert.SerializeObject(data)
    //    });

    //    var context = Substitute.For<IJobExecutionContext>();

    //    context.MergedJobDataMap.Returns(map);

    //    _reminderService.DeleteAsync(userId, reminderId).Returns(DefaultErrors.EntityNotFound<Reminder>());

    //    await _sut.Execute(context);

    //    await _reminderService.Received().DeleteAsync(userId, reminderId);

    //    _logger.Received().Log(
    //        LogLevel.Error,
    //        Arg.Any<EventId>(),
    //        Arg.Any<object>(),
    //        Arg.Any<Exception>(),
    //        Arg.Any<Func<object, Exception?, string>>()
    //    );
    //}
}
