using Eclipse.Application.Jobs;
using Eclipse.Application.MoodRecords.Collection;
using Eclipse.Domain.Users;
using Eclipse.Tests.Extensions;
using Eclipse.Tests.Generators;

using FluentAssertions;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using Quartz;

using Telegram.Bot.Exceptions;

using Xunit;

using User = Eclipse.Domain.Users.User;

namespace Eclipse.Application.Tests.MoodRecords.CollectMoodRecords;

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

        _logger.ShouldReceiveLog(LogLevel.Error);
    }

    [Fact]
    public async Task Execute_WhenDataProvided_ThenCallsCollector()
    {
        var context = Substitute.For<IJobExecutionContext>();

        var userId = Guid.NewGuid();

        var map = new JobDataMap
        {
            { "data", JsonConvert.SerializeObject(new UserIdJobData(userId)) }
        };

        context.MergedJobDataMap.Returns(map);

        await _sut.Execute(context);

        await _collector.Received().CollectAsync(userId);
    }

    [Fact]
    public async Task Execute_WhenApiThrowsException_ThenDisablesUser()
    {
        var context = Substitute.For<IJobExecutionContext>();

        var user = UserGenerator.Get();
        _userRepository.FindAsync(user.Id).Returns(user);

        var map = new JobDataMap
        {
            { "data", JsonConvert.SerializeObject(new UserIdJobData(user.Id)) }
        };

        context.MergedJobDataMap.Returns(map);

        _collector.CollectAsync(user.Id).ThrowsAsync(new ApiRequestException("message", 400));

        await _sut.Execute(context);

        _logger.ShouldReceiveLog(LogLevel.Error);
        await _userRepository.Received().UpdateAsync(user);
        user.IsEnabled.Should().BeFalse();
    }

    [Fact]
    public async Task Execute_WhenApiThrowsExceptionAndUserNotFound_ThenOnlyLogsException()
    {
        var context = Substitute.For<IJobExecutionContext>();

        var userId = Guid.NewGuid();

        var map = new JobDataMap
        {
            { "data", JsonConvert.SerializeObject(new UserIdJobData(userId)) }
        };

        context.MergedJobDataMap.Returns(map);

        _collector.CollectAsync(userId).ThrowsAsync(new ApiRequestException("message", 400));

        await _sut.Execute(context);

        _logger.ShouldReceiveLog(LogLevel.Error);
        await _userRepository.DidNotReceive().UpdateAsync(Arg.Any<User>());
    }
}
