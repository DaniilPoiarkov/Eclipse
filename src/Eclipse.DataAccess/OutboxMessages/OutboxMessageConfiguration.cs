using Eclipse.DataAccess.Cosmos;
using Eclipse.Domain.OutboxMessages;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eclipse.DataAccess.OutboxMessages;

internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.HasPartitionKey(m => m.Id);

        builder.HasKeyAsJsonId();
    }
}
