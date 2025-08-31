using Eclipse.DataAccess.Cosmos;
using Eclipse.Domain.Feedbacks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;

namespace Eclipse.DataAccess.Feedbacks;

internal sealed class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
{
    private readonly IOptions<CosmosDbContextOptions> _options;

    public FeedbackConfiguration(IOptions<CosmosDbContextOptions> options)
    {
        _options = options;
    }

    public void Configure(EntityTypeBuilder<Feedback> builder)
    {
        builder.ToContainer(_options.Value.Container)
            .HasPartitionKey(u => u.Id);

        builder.HasDiscriminator<string>("Discriminator")
            .HasValue(nameof(Feedback));

        builder.HasDiscriminatorInJsonId();
    }
}
