using Eclipse.Application.MoodRecords.Jobs;
using Eclipse.Common.Clock;
using Eclipse.Common.Excel;
using Eclipse.Common.Telegram;
using Eclipse.Domain.MoodRecords;
using Eclipse.Domain.Shared.MoodRecords;

using Microsoft.Extensions.Options;

using NSubstitute;

using Quartz;

using System.Linq.Expressions;

using Telegram.Bot;
using Telegram.Bot.Requests;

using Xunit;

namespace Eclipse.Application.Tests.MoodRecords;

public sealed class ArchiveMoodRecordsJobTests
{
    private readonly IMoodRecordRepository _repository;

    private readonly IExcelManager _excelManager;

    private readonly ITelegramBotClient _telegramBotClient;

    private readonly ITimeProvider _timeProvider;

    private readonly IOptions<TelegramOptions> _options;

    private readonly ArchiveMoodRecordsJob _sut;

    public ArchiveMoodRecordsJobTests()
    {
        _repository = Substitute.For<IMoodRecordRepository>();
        _excelManager = Substitute.For<IExcelManager>();
        _telegramBotClient = Substitute.For<ITelegramBotClient>();
        _timeProvider = Substitute.For<ITimeProvider>();
        _options = Substitute.For<IOptions<TelegramOptions>>();

        _sut = new ArchiveMoodRecordsJob(_repository, _excelManager, _telegramBotClient, _timeProvider, _options);
    }

    [Fact]
    public async Task Execute_WhenTriggered_ThenArchivesRecords()
    {
        var utcNow = DateTime.UtcNow;

        _timeProvider.Now.Returns(utcNow);

        var records = new[]
        {
            new MoodRecord(Guid.NewGuid(), Guid.NewGuid(), MoodState.Neutral, DateTime.UtcNow)
        };

        _repository.GetByExpressionAsync(Arg.Any<Expression<Func<MoodRecord, bool>>>())
            .Returns(records);

        using var stream = new MemoryStream();

        _excelManager.Write(records).Returns(stream);

        var options = new TelegramOptions
        {
            Chat = 1
        };

        _options.Value.Returns(options);

        var context = Substitute.For<IJobExecutionContext>();

        await _sut.Execute(context);

        await _repository.Received().GetByExpressionAsync(Arg.Any<Expression<Func<MoodRecord, bool>>>());
        _excelManager.Received().Write(records);

        await _telegramBotClient.Received().SendRequest(
            Arg.Is<SendDocumentRequest>(r => r.ChatId == options.Chat && r.Caption == "Mood records archive")
        );

        await _repository.Received().DeleteRangeAsync(records);
    }
}
