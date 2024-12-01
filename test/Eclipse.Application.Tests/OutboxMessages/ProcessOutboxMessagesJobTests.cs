using Eclipse.Application.Contracts.OutboxMessages;
using Eclipse.Application.OutboxMessages.Jobs;

using Microsoft.Extensions.Configuration;

using NSubstitute;

using Quartz;

using Xunit;

namespace Eclipse.Application.Tests.OutboxMessages;

public sealed class ProcessOutboxMessagesJobTests
{
    private readonly IOutboxMessagesService _outboxMessagesService;

    private readonly IConfiguration _configuration;

    private readonly ProcessOutboxMessagesJob _sut;

    public ProcessOutboxMessagesJobTests()
    {
        _outboxMessagesService = Substitute.For<IOutboxMessagesService>();
        _configuration = Substitute.For<IConfiguration>();

        _sut = new ProcessOutboxMessagesJob(_outboxMessagesService, _configuration);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public async Task Execute_WhenTriggered_ThenRetrievesConfigurationAndCallsService(int count)
    {
        var section = Substitute.For<IConfigurationSection>();
        section.GetSection("ProcessCount").Value.Returns($"{count}");

        _configuration.GetSection("OutboxMessages").Returns(section);

        var context = Substitute.For<IJobExecutionContext>();

        await _sut.Execute(context);

        _configuration.Received().GetSection("OutboxMessages");
        section.Received().GetValue<int>("ProcessCount");

        await _outboxMessagesService.Received().ProcessAsync(count);
    }
}
