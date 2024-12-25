using Eclipse.DataAccess.CosmosDb;
using Eclipse.Domain.InboxMessages;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;

namespace Eclipse.DataAccess.Configurations;

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
    }
}
