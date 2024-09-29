using Eclipse.Common.Clock;
using Eclipse.Common.Excel;
using Eclipse.Common.Telegram;
using Eclipse.Domain.MoodRecords;
using Eclipse.Domain.Shared.Repositories;

using Microsoft.Extensions.Options;

using Quartz;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Application.MoodRecords.Jobs;

internal sealed class ArchiveMoodRecordsJob : IJob
{
    private readonly IRepository<MoodRecord> _repository;

    private readonly IExcelManager _excelManager;

    private readonly ITelegramBotClient _telegramBotClient;

    private readonly ITimeProvider _timeProvider;

    private readonly IOptions<TelegramOptions> _options;

    private static readonly TimeOnly _endOfDay = new(23, 59);

    public ArchiveMoodRecordsJob(
        IRepository<MoodRecord> repository,
        IExcelManager excelManager,
        ITelegramBotClient telegramBotClient,
        ITimeProvider timeProvider,
        IOptions<TelegramOptions> options)
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
            .PreviousDayOfWeek(DayOfWeek.Sunday)
            .WithTime(_endOfDay);

        var records = await _repository.GetByExpressionAsync(
            mr => mr.CreatedAt < date,
            context.CancellationToken
        );

        using var stream = _excelManager.Write(records);

        await _telegramBotClient.SendDocumentAsync(
            _options.Value.Chat,
            InputFile.FromStream(stream, $"mood-records-archive-{date:dd.MM.yyyy}.xlsx"),
            caption: "Mood records archive",
            cancellationToken: context.CancellationToken
        );

        await _repository.DeleteRangeAsync(records, context.CancellationToken);
    }
}
