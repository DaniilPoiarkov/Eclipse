using Eclipse.Infrastructure.Telegram;

using Quartz;

namespace Eclipse.Application.Quartz.Jobs;

internal class TestJob : IJob
{
    private readonly ITelegramService _telegramService;

    public TestJob(ITelegramService telegramService)
    {
        _telegramService = telegramService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var data = context.JobDetail.JobDataMap;

        var id = data.GetLong("id");

        await _telegramService.Send(new SendMessageModel
        {
            ChatId = id,
            Message = "Test"
        });
    }
}
