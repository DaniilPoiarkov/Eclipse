using Eclipse.Common.Specifications;

using System.Linq.Expressions;

namespace Eclipse.Domain.OutboxMessages;

public sealed class OutboxMessageNotProcessedSpecification : Specification<OutboxMessage>
{
    public override Expression<Func<OutboxMessage, bool>> IsSatisfied()
    {
        return outboxMessage => outboxMessage.Error == null && outboxMessage.ProcessedAt == null;
    }
}
