using Eclipse.Application.Contracts.InboxMessages;
using Eclipse.Application.InboxMessages;

using Microsoft.Extensions.Configuration;

using NSubstitute;

using Quartz;

using Xunit;

namespace Eclipse.Application.Tests.InboxMessages;

public sealed class ConvertOutboxToInboxJobTests
{
    private readonly IInboxMessageConvertor _inboxMessageConvertor;

    private readonly IConfiguration _configuration;

    private readonly ConvertOutboxToInboxJob _sut;

    public ConvertOutboxToInboxJobTests()
    {
        _inboxMessageConvertor = Substitute.For<IInboxMessageConvertor>();
        _configuration = Substitute.For<IConfiguration>();

        _sut = new ConvertOutboxToInboxJob(_inboxMessageConvertor, _configuration);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public async Task Execute_WhenTriggered_ThenRetrievesConfigurationAndCallsService(int count)
    {
        var section = Substitute.For<IConfigurationSection>();
        section.GetSection("ConvertCount").Value.Returns($"{count}");

        _configuration.GetSection("InboxMessages").Returns(section);

        var context = Substitute.For<IJobExecutionContext>();

        await _sut.Execute(context);

        _configuration.Received().GetSection("InboxMessages");
        section.Received().GetValue<int>("ConvertCount");

        await _inboxMessageConvertor.Received().ConvertAsync(count);
    }
}
