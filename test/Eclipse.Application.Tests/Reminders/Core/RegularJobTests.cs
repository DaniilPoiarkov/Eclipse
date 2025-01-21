using Eclipse.Application.Reminders.Core;

using FluentAssertions;

using Microsoft.Extensions.Logging;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using Quartz;
using Xunit;

namespace Eclipse.Application.Tests.Reminders.Core;

public sealed class RegularJobTests
{
    private readonly INotificationJob<object> _job;

    private readonly ILogger<RegularJob<INotificationJob<object>, object>> _logger;

    private readonly RegularJob<INotificationJob<object>, object> _sut;

    public RegularJobTests()
    {
        _job = Substitute.For<INotificationJob<object>>();
        _logger = Substitute.For<ILogger<RegularJob<INotificationJob<object>, object>>>();
        _sut = new RegularJob<INotificationJob<object>, object>(_logger, _job);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Execute_WhenDataInvalid_ThenLogsError(string? data)
    {
        var context = Substitute.For<IJobExecutionContext>();

        var dataMap = new JobDataMap
        {
            { "data", data! }
        };

        context.MergedJobDataMap.Returns(dataMap);

        await _sut.Execute(context);

        _logger.Received().Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>()
        );
    }

    [Theory]
    [InlineData(5)]
    public async Task Execute_WhenExceededRefireCount_ThenNotProcess(int refireCount)
    {
        var context = Substitute.For<IJobExecutionContext>();
        context.RefireCount.Returns(refireCount);

        await _sut.Execute(context);

        _logger.Received().Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>()
        );

        await _job.DidNotReceive().Handle(Arg.Any<object>());
    }

    [Fact]
    public async Task Execute_WhenJobFails_ThenThrowsJobExecutionException()
    {
        var context = Substitute.For<IJobExecutionContext>();

        var dataMap = new JobDataMap
        {
            { "data", "{}" }
        };

        context.MergedJobDataMap.Returns(dataMap);

        _job.Handle(Arg.Any<object>()).Throws<InvalidOperationException>();

        var action = () => _sut.Execute(context);

        await action.Should().ThrowAsync<JobExecutionException>()
            .WithMessage($"Failed to process {typeof(INotificationJob<object>).Name} job.")
            .WithInnerException(typeof(InvalidOperationException));
    }
}
