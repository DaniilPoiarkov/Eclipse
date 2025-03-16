using Eclipse.Application.MoodRecords.Collection;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using NSubstitute;

using Quartz;

using Xunit;

namespace Eclipse.Application.Tests.Reminders.CollectMoodRecords;

public sealed class CollectMoodRecordJobTests
{
    private readonly IMoodRecordCollector _collector;

    private readonly ILogger<CollectMoodRecordJob> _logger;

    private readonly CollectMoodRecordJob _sut;

    public CollectMoodRecordJobTests()
    {
        _collector = Substitute.For<IMoodRecordCollector>();
        _logger = Substitute.For<ILogger<CollectMoodRecordJob>>();

        _sut = new CollectMoodRecordJob(_collector, _logger);
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
}
