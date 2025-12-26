using Eclipse.Domain.InboxMessages;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eclipse.DataAccess.InboxMessages;

internal sealed class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        builder.HasPartitionKey(m => m.Id);

        builder.HasDiscriminator<string>("Discriminator")
            .HasValue(nameof(InboxMessage));

        builder.HasDiscriminatorInJsonId();
    }
}
