using Eclipse.Application.MoodRecords.Collection;
using Eclipse.Domain.Users;
using Eclipse.Tests.Extensions;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using Quartz;

using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

using Xunit;

namespace Eclipse.Application.Tests.Reminders.CollectMoodRecords;

public sealed class CollectMoodRecordJobTests
{
    private readonly IMoodRecordCollector _collector;

    private readonly IUserRepository _userRepository;

    private readonly ILogger<CollectMoodRecordJob> _logger;

    private readonly CollectMoodRecordJob _sut;

    public CollectMoodRecordJobTests()
    {
        _collector = Substitute.For<IMoodRecordCollector>();
        _userRepository = Substitute.For<IUserRepository>();
        _logger = Substitute.For<ILogger<CollectMoodRecordJob>>();

        _sut = new CollectMoodRecordJob(_collector, _userRepository, _logger);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("\"\"")]
    public async Task Execute_WhenDataIsInvalid_ThenErrorLogged(string? data)
    {
        var context = Substitute.For<IJobExecutionContext>();

        var map = new JobDataMap
        {
            { "data", data! }
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
    public async Task Execute_WhenDataProvided_ThenCallsCollector()
    {
        var context = Substitute.For<IJobExecutionContext>();

        var userId = Guid.NewGuid();

        var map = new JobDataMap
        {
            { "data", JsonConvert.SerializeObject(new CollectMoodRecordJobData(userId)) }
        };

        context.MergedJobDataMap.Returns(map);

        await _sut.Execute(context);

        await _collector.Received().CollectAsync(userId);
    }

    [Fact]
    public async Task Execute_WhenApiThrowsException_ThenDisablesUser()
    {
        var context = Substitute.For<IJobExecutionContext>();

        var userId = Guid.NewGuid();

        var map = new JobDataMap
        {
            { "data", JsonConvert.SerializeObject(new CollectMoodRecordJobData(userId)) }
        };

        context.MergedJobDataMap.Returns(map);

        _collector.CollectAsync(userId).ThrowsAsync(new ApiRequestException("message", 400));

        await _sut.Execute(context);

        _logger.ShouldReceiveLog(LogLevel.Error);
    }
}
