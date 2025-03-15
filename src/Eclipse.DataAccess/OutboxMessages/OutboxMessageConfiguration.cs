using Eclipse.DataAccess.Cosmos;
using Eclipse.Domain.OutboxMessages;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;

namespace Eclipse.DataAccess.OutboxMessages;

internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    private readonly IOptions<CosmosDbContextOptions> _options;

    public OutboxMessageConfiguration(IOptions<CosmosDbContextOptions> options)
    {
        _options = options;
    }

    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToContainer(_options.Value.Container)
            .HasPartitionKey(m => m.Id);

        builder.HasDiscriminator<string>("Discriminator")
            .HasValue(nameof(OutboxMessage));

        builder.HasDiscriminatorInJsonId();
    }
}
