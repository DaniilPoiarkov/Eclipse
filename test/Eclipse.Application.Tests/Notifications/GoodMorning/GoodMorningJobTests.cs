﻿using Eclipse.Application.Notifications.GoodMorning;
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

namespace Eclipse.Application.Tests.Notifications.GoodMorning;

public sealed class GoodMorningJobTests
{
    private readonly ICurrentCulture _currentCulture;

    private readonly IStringLocalizer<GoodMorningJob> _localizer;

    private readonly ITelegramBotClient _client;

    private readonly IUserRepository _repository;

    private readonly ILogger<GoodMorningJob> _logger;

    private readonly GoodMorningJob _sut;

    public GoodMorningJobTests()
    {
        _currentCulture = Substitute.For<ICurrentCulture>();
        _localizer = Substitute.For<IStringLocalizer<GoodMorningJob>>();
        _client = Substitute.For<ITelegramBotClient>();
        _repository = Substitute.For<IUserRepository>();
        _logger = Substitute.For<ILogger<GoodMorningJob>>();

        _sut = new GoodMorningJob(_currentCulture, _localizer, _client, _repository, _logger);
    }

    [Fact]
    public async Task Execute_WhenUserNotFound_ThenLogsError()
    {
        _repository.FindAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<User?>(null));

        var context = Substitute.For<IJobExecutionContext>();

        var map = new JobDataMap
        {
            { "data", JsonConvert.SerializeObject(new GoodMorningJobData(Guid.NewGuid())) }
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
    public async Task Execute_WhenUserFound_ThenSendsNotification()
    {
        var user = UserGenerator.Get();

        _repository.FindAsync(user.Id).Returns(user);

        _localizer[Arg.Any<string>()].Returns(new LocalizedString("", "good morning"));

        var context = Substitute.For<IJobExecutionContext>();

        var map = new JobDataMap
        {
            { "data", JsonConvert.SerializeObject(new GoodMorningJobData(user.Id)) }
        };

        context.MergedJobDataMap.Returns(map);

        await _sut.Execute(context);

        _currentCulture.Received().UsingCulture(user.Culture);

        await _client.Received().SendRequest(
            Arg.Is<SendMessageRequest>(r => r.ChatId == user.ChatId && r.Text == "good morning")
        );
    }

    [Fact]
    public async Task Execute_WhenClientThrowsException_ThenDisablesUser()
    {
        var user = UserGenerator.Get();

        _repository.FindAsync(user.Id).Returns(user);

        _localizer[Arg.Any<string>()].Returns(new LocalizedString("", "good morning"));

        var context = Substitute.For<IJobExecutionContext>();

        var map = new JobDataMap
        {
            { "data", JsonConvert.SerializeObject(new GoodMorningJobData(user.Id)) }
        };

        context.MergedJobDataMap.Returns(map);

        _client.SendRequest(Arg.Any<SendMessageRequest>())
            .Throws(new ApiRequestException("test"));

        await _sut.Execute(context);

        user.IsEnabled.Should().BeFalse();
        await _repository.Received().UpdateAsync(user);
        _logger.ShouldReceiveLog(LogLevel.Error);
    }
}
