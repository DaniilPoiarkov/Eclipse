using Eclipse.Domain.ApiTokens;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eclipse.DataAccess.ApiTokens;

internal sealed class ApiTokenConfiguration : IEntityTypeConfiguration<ApiToken>
{
    public void Configure(EntityTypeBuilder<ApiToken> builder)
    {
        builder.HasPartitionKey(t => t.Id);

        builder.HasDiscriminator<string>("Discriminator")
            .HasValue(nameof(ApiToken));

        builder.HasDiscriminatorInJsonId();
    }
}
