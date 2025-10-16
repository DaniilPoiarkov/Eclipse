using Eclipse.Application.Contracts.Reports;
using Eclipse.Application.MoodRecords.Report;
using Eclipse.Domain.Users;
using Eclipse.Localization.Culture;
using Eclipse.Tests.Extensions;
using Eclipse.Tests.Generators;

using FluentAssertions;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Requests;
using Telegram.Bot.Types.Enums;

using Xunit;

namespace Eclipse.Application.Tests.MoodRecords.Report;

public sealed class MoodReportSenderTests
{
    private readonly IUserRepository _repository;

    private readonly ICurrentCulture _currentCulture;

    private readonly IReportsService _reportsService;

    private readonly ITelegramBotClient _client;

    private readonly IStringLocalizer<MoodReportSender> _localizer;

    private readonly ILogger<MoodReportSender> _logger;

    private readonly MoodReportSender _sut;

    public MoodReportSenderTests()
    {
        _repository = Substitute.For<IUserRepository>();
        _currentCulture = Substitute.For<ICurrentCulture>();
        _reportsService = Substitute.For<IReportsService>();
        _client = Substitute.For<ITelegramBotClient>();
        _localizer = Substitute.For<IStringLocalizer<MoodReportSender>>();
        _logger = Substitute.For<ILogger<MoodReportSender>>();

        _sut = new MoodReportSender(_repository, _currentCulture, _reportsService, _client, _localizer, _logger);
    }

    [Fact]
    public async Task Execute_WhenUserNotFound_ThenLogsError()
    {
        var options = new SendMoodReportOptions(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow, "");

        await _sut.Send(Guid.NewGuid(), options);

        _logger.ShouldReceiveLog(LogLevel.Error);
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<User>());
    }

    [Fact]
    public async Task Execute_WhenUserFound_ThenSendsNotification()
    {
        var user = UserGenerator.Get();

        _repository.FindAsync(user.Id).Returns(user);
        _localizer[Arg.Any<string>()].Returns(new LocalizedString("", "mood report"));

        using var stream = new MemoryStream();
        _reportsService.GetMoodReportAsync(user.Id, Arg.Any<MoodReportOptions>()).Returns(stream);

        var options = new SendMoodReportOptions(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow, "");

        await _sut.Send(user.Id, options);

        _currentCulture.Received().UsingCulture(user.Culture);

        await _reportsService.Received().GetMoodReportAsync(user.Id,
            Arg.Is<MoodReportOptions>(o => o.From == options.From
                && o.To == options.To)
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

        var exception = new ApiRequestException("test");
        _client.SendRequest(Arg.Any<SendPhotoRequest>()).ThrowsAsync(exception);

        using var stream = new MemoryStream();
        _reportsService.GetMoodReportAsync(user.Id, Arg.Any<MoodReportOptions>()).Returns(stream);

        var options = new SendMoodReportOptions(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow, "");

        await _sut.Send(user.Id, options);

        _logger.ShouldReceiveLog(LogLevel.Error);
        user.IsEnabled.Should().BeFalse();
        await _repository.Received().UpdateAsync(user);
    }
}
