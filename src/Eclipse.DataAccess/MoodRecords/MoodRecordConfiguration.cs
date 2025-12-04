using Eclipse.DataAccess.Cosmos;
using Eclipse.Domain.MoodRecords;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;

namespace Eclipse.DataAccess.MoodRecords;

internal sealed class MoodRecordConfiguration : IEntityTypeConfiguration<MoodRecord>
{
    private readonly IOptions<CosmosDbContextOptions> _options;

    public MoodRecordConfiguration(IOptions<CosmosDbContextOptions> options)
    {
        _options = options;
    }

    public void Configure(EntityTypeBuilder<MoodRecord> builder)
    {
        builder.ToContainer(_options.Value.Container)
            .HasPartitionKey(mr => mr.Id);

        builder.HasDiscriminator<string>("Discriminator")
            .HasValue(nameof(MoodRecord));

        builder.HasDiscriminatorInJsonId();
    }
}
