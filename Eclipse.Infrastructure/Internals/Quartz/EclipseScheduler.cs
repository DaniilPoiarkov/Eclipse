using Eclipse.Infrastructure.Quartz;
using Eclipse.Infrastructure.Telegram;

using Quartz;
using Quartz.Impl.Matchers;

namespace Eclipse.Infrastructure.Internals.Quartz;

internal class EclipseScheduler : IEclipseScheduler
{
    private readonly ISchedulerFactory _schedulerFactory;

    public EclipseScheduler(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    public async Task Test(long id)
    {
        var sched = await _schedulerFactory.GetScheduler();
        
        var keys = await sched.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());

        var key = JobKey.Create(nameof(TestJob), $"{id}");

        var job = JobBuilder.Create<TestJob>().WithIdentity(key)
            .UsingJobData("id", id)
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithSimpleSchedule(s => s.WithIntervalInSeconds(5).RepeatForever())
            .StartNow()
            .Build();

        await sched.ScheduleJob(job, trigger);

        await Task.Delay(11_000);

        await sched.DeleteJob(key);
    }

    private class TestJob : IJob
    {
        private readonly ITelegramService _telegramService;

        public TestJob(ITelegramService telegramService)
        {
            _telegramService = telegramService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var id = context.JobDetail.JobDataMap.GetLong("id");

            await _telegramService.Send(new SendMessageModel
            {
                ChatId = id,
                Message = "Test"
            }, context.CancellationToken);
        }
    }
}
