using Eclipse.DataAccess.CosmosDb;
using Eclipse.Domain.Statistics;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;

namespace Eclipse.DataAccess.Configurations;

internal sealed class UserStatisticsConfiguration : IEntityTypeConfiguration<UserStatistics>
{
    private readonly IOptions<CosmosDbContextOptions> _options;

    public UserStatisticsConfiguration(IOptions<CosmosDbContextOptions> options)
    {
        _options = options;
    }

    public void Configure(EntityTypeBuilder<UserStatistics> builder)
    {
        builder.ToContainer(_options.Value.Container)
            .HasPartitionKey(us => us.Id);

        builder.HasDiscriminator<string>("Discriminator")
            .HasValue(nameof(UserStatistics));

        builder.HasDiscriminatorInJsonId();
    }
}
