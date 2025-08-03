using Eclipse.Application.InboxMessages;
using Eclipse.Domain.InboxMessages;
using Eclipse.Domain.Shared.InboxMessages;

using FluentAssertions;

using NSubstitute;

using System.Linq.Expressions;

using Xunit;

namespace Eclipse.Application.Tests.InboxMessages;

public sealed class InboxMessageServiceTests
{
    private readonly IInboxMessageRepository _repository;

    private readonly InboxMessageService _sut;

    public InboxMessageServiceTests()
    {
        _repository = Substitute.For<IInboxMessageRepository>();
        _sut = new InboxMessageService(_repository);
    }

    [Fact]
    public async Task DeleteProcessedAsync_WhenCalled_ThenDelegatesCallToRepository()
    {
        await _sut.DeleteProcessedAsync();
        await _repository.Received().DeleteSuccessfullyProcessedAsync();
    }

    [Fact]
    public async Task ResetFailedAsync_WhenCalled_ThenResetsFailedMessages()
    {
        var messages = new List<InboxMessage>
        {
            InboxMessage.Create(Guid.NewGuid(), Guid.NewGuid(), "handler", "{}", "object", DateTime.UtcNow),
            InboxMessage.Create(Guid.NewGuid(), Guid.NewGuid(), "handler", "{}", "object", DateTime.UtcNow)
        };

        foreach (var message in messages)
        {
            message.SetError("error", DateTime.UtcNow);
        }

        _repository.GetByExpressionAsync(Arg.Any<Expression<Func<InboxMessage, bool>>>()).Returns(messages);

        await _sut.ResetFailedAsync();

        await _repository.Received().GetByExpressionAsync(Arg.Any<Expression<Func<InboxMessage, bool>>>());
        await _repository.Received().UpdateRangeAsync(messages);

        messages.Should().AllSatisfy(m =>
        {
            m.Status.Should().Be(InboxMessageStatus.Pending);
            m.Error.Should().Be("error");
            m.ProcessedAt.Should().NotBeNull();
        }, "Expected inbox message reset contain previous error in case of any tracking.");
    }
}
