using Eclipse.DataAccess.Cosmos;
using Eclipse.Domain.Feedbacks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eclipse.DataAccess.Feedbacks;

internal sealed class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
{
    public void Configure(EntityTypeBuilder<Feedback> builder)
    {
        builder.HasPartitionKey(u => u.Id);

        builder.HasKeyAsJsonId();
    }
}
