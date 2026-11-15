using Eclipse.DataAccess.Cosmos;
using Eclipse.Domain.PromotionLogs;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eclipse.DataAccess.PromotionLogs;

internal sealed class PromotionLogConfiguration : IEntityTypeConfiguration<PromotionLog>
{
    public void Configure(EntityTypeBuilder<PromotionLog> builder)
    {
        builder.HasPartitionKey(u => u.Id);

        builder.HasKeyAsJsonId();
    }
}
