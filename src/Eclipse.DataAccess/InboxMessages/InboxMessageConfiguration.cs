using Eclipse.DataAccess.Cosmos;
using Eclipse.Domain.InboxMessages;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;

namespace Eclipse.DataAccess.InboxMessages;

internal sealed class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>
{
    private readonly IOptions<CosmosDbContextOptions> _options;

    public InboxMessageConfiguration(IOptions<CosmosDbContextOptions> options)
    {
        _options = options;
    }

    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        builder.ToContainer(_options.Value.Container)
            .HasPartitionKey(m => m.Id);

        builder.HasDiscriminator<string>("Discriminator")
            .HasValue(nameof(InboxMessage));

        builder.HasDiscriminatorInJsonId();
    }
}
