using Eclipse.Application.Contracts.Reports;
using Eclipse.Application.Jobs;
using Eclipse.Application.MoodRecords.Report.Weekly;
using Eclipse.Common.Clock;
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
using Telegram.Bot.Types.Enums;

using Xunit;

namespace Eclipse.Application.Tests.MoodRecords.MoodReport;

public sealed class MoodReportJobTests
{
    private readonly IUserRepository _repository;

    private readonly ICurrentCulture _currentCulture;

    private readonly IReportsService _reportsService;

    private readonly ITelegramBotClient _client;

    private readonly ITimeProvider _timeProvider;

    private readonly IStringLocalizer<WeeklyMoodReportJob> _localizer;

    private readonly ILogger<WeeklyMoodReportJob> _logger;

    private readonly WeeklyMoodReportJob _sut;

    public MoodReportJobTests()
    {
        _repository = Substitute.For<IUserRepository>();
        _currentCulture = Substitute.For<ICurrentCulture>();
        _reportsService = Substitute.For<IReportsService>();
        _client = Substitute.For<ITelegramBotClient>();
        _timeProvider = Substitute.For<ITimeProvider>();
        _localizer = Substitute.For<IStringLocalizer<WeeklyMoodReportJob>>();
        _logger = Substitute.For<ILogger<WeeklyMoodReportJob>>();

        _sut = new WeeklyMoodReportJob(_repository, _currentCulture, _reportsService, _client, _timeProvider, _localizer, _logger);
    }

    [Fact]
    public async Task Execute_WhenUserNotFound_ThenLogsError()
    {
        var context = Substitute.For<IJobExecutionContext>();

        var map = new JobDataMap
        {
            { "data", JsonConvert.SerializeObject(new UserIdJobData(Guid.NewGuid())) }
        };

        context.MergedJobDataMap.Returns(map);

        await _sut.Execute(context);

        _logger.ShouldReceiveLog(LogLevel.Error);
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<User>());
    }

    [Fact]
    public async Task Execute_WhenUserFound_ThenSendsNotification()
    {
        var user = UserGenerator.Get();

        _repository.FindAsync(user.Id).Returns(user);

        _localizer[Arg.Any<string>()].Returns(new LocalizedString("", "mood report"));

        var utcNow = DateTime.UtcNow;
        _timeProvider.Now.Returns(utcNow);

        var from = utcNow.PreviousDayOfWeek(DayOfWeek.Sunday).WithTime(0, 0);

        using var stream = new MemoryStream();
        _reportsService.GetMoodReportAsync(user.Id, Arg.Any<MoodReportOptions>()).Returns(stream);

        var context = Substitute.For<IJobExecutionContext>();

        var map = new JobDataMap
        {
            { "data", JsonConvert.SerializeObject(new UserIdJobData(user.Id)) }
        };

        context.MergedJobDataMap.Returns(map);

        await _sut.Execute(context);

        _currentCulture.Received().UsingCulture(user.Culture);

        await _reportsService.Received().GetMoodReportAsync(user.Id,
            Arg.Is<MoodReportOptions>(o => o.From == from && o.To == utcNow)
        );

        await _client.Received().SendRequest(
            Arg.Is<SendPhotoRequest>(r => r.ChatId == user.ChatId
                && r.Caption == "mood report"
                && r.Photo.FileType == FileType.Stream)
        );
    }

    [Fact]
    public async Task Execute_WhenApiThrowsException_ThenLogsErrorAndDisablesUser()
    {
        var user = UserGenerator.Get();
        _repository.FindAsync(user.Id).Returns(user);

        _timeProvider.Now.Returns(DateTime.UtcNow);

        var exception = new ApiRequestException("test");
        _client.SendRequest(Arg.Any<SendPhotoRequest>()).ThrowsAsync(exception);

        using var stream = new MemoryStream();
        _reportsService.GetMoodReportAsync(user.Id, Arg.Any<MoodReportOptions>()).Returns(stream);

        var context = Substitute.For<IJobExecutionContext>();

        var map = new JobDataMap
        {
            { "data", JsonConvert.SerializeObject(new UserIdJobData(user.Id)) }
        };

        context.MergedJobDataMap.Returns(map);

        await _sut.Execute(context);

        _logger.ShouldReceiveLog(LogLevel.Error);
        user.IsEnabled.Should().BeFalse();
        await _repository.Received().UpdateAsync(user);
    }
}
