using Eclipse.DataAccess.Constants;
using Eclipse.Domain.OutboxMessages;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eclipse.DataAccess.Configurations;

internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToContainer(ContainerNames.Aggregates)
            .HasPartitionKey(m => m.Id);
    }
}
