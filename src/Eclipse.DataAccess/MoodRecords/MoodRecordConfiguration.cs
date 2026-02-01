using Eclipse.Domain.MoodRecords;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eclipse.DataAccess.MoodRecords;

internal sealed class MoodRecordConfiguration : IEntityTypeConfiguration<MoodRecord>
{
    public void Configure(EntityTypeBuilder<MoodRecord> builder)
    {
        builder.HasPartitionKey(mr => mr.Id);

        builder.HasDiscriminator<string>("Discriminator")
            .HasValue(nameof(MoodRecord));

        builder.HasDiscriminatorInJsonId();
    }
}
