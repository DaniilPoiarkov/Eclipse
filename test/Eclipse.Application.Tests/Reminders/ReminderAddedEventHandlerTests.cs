using Eclipse.Application.Reminders;
using Eclipse.Application.Reminders.Sendings;
using Eclipse.Common.Clock;
using Eclipse.Domain.Users.Events;

using NSubstitute;

using Quartz;

using Xunit;

namespace Eclipse.Application.Tests.Reminders;

public sealed class ReminderAddedEventHandlerTests
{
    private readonly IScheduler _scheduler;

    private readonly ISchedulerFactory _schedulerFactory;

    private readonly ITimeProvider _timeProvider;

    private readonly ReminderAddedEventHandler _sut;

    public ReminderAddedEventHandlerTests()
    {
        _schedulerFactory = Substitute.For<ISchedulerFactory>();
        _timeProvider = Substitute.For<ITimeProvider>();
        _scheduler = Substitute.For<IScheduler>();

        _schedulerFactory.GetScheduler().Returns(_scheduler);

        _sut = new ReminderAddedEventHandler(_schedulerFactory, _timeProvider);
    }

    [Theory]
    [InlineData("test", "en", 1)]
    public async Task Handle_WhenTriggered_ThenSchedulesJob(string text, string culture, long chatId)
    {
        _timeProvider.Now.Returns(DateTime.UtcNow);

        var reminderId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var notification = new ReminderAddedDomainEvent(reminderId, userId, null, new TimeSpan(), new TimeOnly(), text, culture, chatId);

        await _sut.Handle(notification, CancellationToken.None);

        await _schedulerFactory.Received().GetScheduler();

        await _scheduler.ScheduleJob(
            Arg.Is<IJobDetail>(detail => detail.Key.Name == $"{nameof(SendReminderJob)}-{userId}-{reminderId}"),
            Arg.Is<ITrigger>(trigger => trigger.JobKey.Name == $"{nameof(SendReminderJob)}-{userId}-{reminderId}")
        );
    }
}
