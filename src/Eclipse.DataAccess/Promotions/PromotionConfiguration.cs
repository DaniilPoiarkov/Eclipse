using Eclipse.Domain.Promotions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eclipse.DataAccess.Promotions;

internal sealed class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
{
    public void Configure(EntityTypeBuilder<Promotion> builder)
    {
        builder.HasPartitionKey(p => p.Id);

        builder.HasDiscriminator<string>("Discriminator")
            .HasValue(nameof(Promotion));

        builder.HasDiscriminatorInJsonId();
    }
}
