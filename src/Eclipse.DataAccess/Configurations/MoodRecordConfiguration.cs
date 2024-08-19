using Eclipse.DataAccess.Constants;
using Eclipse.Domain.MoodRecords;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eclipse.DataAccess.Configurations;

internal class MoodRecordConfiguration : IEntityTypeConfiguration<MoodRecord>
{
    public void Configure(EntityTypeBuilder<MoodRecord> builder)
    {
        builder.ToContainer(ContainerNames.Aggregates)
            .HasPartitionKey(x => x.Id);
    }
}
