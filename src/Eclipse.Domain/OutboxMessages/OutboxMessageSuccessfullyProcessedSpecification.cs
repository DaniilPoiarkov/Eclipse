using Eclipse.Common.Specifications;

using System.Linq.Expressions;

namespace Eclipse.Domain.OutboxMessages;

public sealed class OutboxMessageSuccessfullyProcessedSpecification : Specification<OutboxMessage>
{
    public override Expression<Func<OutboxMessage, bool>> IsSatisfied()
    {
        return outboxMessage => outboxMessage.ProcessedAt != null && outboxMessage.Error == null;
    }
}
