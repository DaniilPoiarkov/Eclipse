using Eclipse.Common.Clock;
using Eclipse.Common.Excel;
using Eclipse.Domain.MoodRecords;

using Microsoft.Extensions.Options;

using Quartz;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Application.MoodRecords.Archiving;

internal sealed class ArchiveMoodRecordsJob : IJob
{
    private readonly IMoodRecordRepository _repository;

    private readonly IExcelManager _excelManager;

    private readonly ITelegramBotClient _telegramBotClient;

    private readonly ITimeProvider _timeProvider;

    private readonly IOptions<ApplicationOptions> _options;

    private static readonly TimeOnly _endOfDay = new(23, 59);

    public ArchiveMoodRecordsJob(
        IMoodRecordRepository repository,
        IExcelManager excelManager,
        ITelegramBotClient telegramBotClient,
        ITimeProvider timeProvider,
        IOptions<ApplicationOptions> options)
    {
        _repository = repository;
        _excelManager = excelManager;
        _telegramBotClient = telegramBotClient;
        _timeProvider = timeProvider;
        _options = options;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var date = _timeProvider.Now
            .PreviousDayOfWeek(_timeProvider.Now.DayOfWeek)
            .WithTime(_endOfDay);

        var records = await _repository.GetByExpressionAsync(
            mr => mr.CreatedAt <= date,
            context.CancellationToken
        );

        using var stream = _excelManager.Write(records);

        await _telegramBotClient.SendDocument(
            _options.Value.Chat,
            InputFile.FromStream(stream, $"mood-records-archive-{date:dd.MM.yyyy}.xlsx"),
            caption: "Mood records archive",
            cancellationToken: context.CancellationToken
        );

        await _repository.DeleteRangeAsync(records, context.CancellationToken);
    }
}
