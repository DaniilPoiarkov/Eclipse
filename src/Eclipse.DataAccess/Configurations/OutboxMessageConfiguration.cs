using Eclipse.DataAccess.CosmosDb;
using Eclipse.Domain.OutboxMessages;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;

namespace Eclipse.DataAccess.Configurations;

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
    }
}
