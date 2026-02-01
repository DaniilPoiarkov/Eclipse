using Eclipse.Domain.Statistics;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eclipse.DataAccess.Statistics;

internal sealed class UserStatisticsConfiguration : IEntityTypeConfiguration<UserStatistics>
{
    public void Configure(EntityTypeBuilder<UserStatistics> builder)
    {
        builder.HasPartitionKey(us => us.Id);

        builder.HasDiscriminator<string>("Discriminator")
            .HasValue(nameof(UserStatistics));

        builder.HasDiscriminatorInJsonId();
    }
}
