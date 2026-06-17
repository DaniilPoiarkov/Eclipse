using Eclipse.Domain.ApiTokens;
using Eclipse.Domain.Shared.ApiTokens;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eclipse.DataAccess.ApiTokens;

internal sealed class ApiTokenConfiguration : IEntityTypeConfiguration<ApiToken>
{
    public void Configure(EntityTypeBuilder<ApiToken> builder)
    {
        builder.HasPartitionKey(t => t.Id);

        builder.HasDiscriminator<string>("Discriminator")
            .HasValue(nameof(ApiToken));

        builder.Property(t => t.Scopes)
            .HasConversion<ApiTokenScopesConversion>(
                new ValueComparer<IReadOnlyList<ApiTokenScope>>(
                    (l, r) =>
                    (l == null && r == null) || (!l.IsNullOrEmpty()
                        && !r.IsNullOrEmpty()
                        && l.SequenceEqual(r)
                    ),
                    scopes => scopes.Aggregate(0, (acc, s) => acc + s.GetHashCode())
                )
            );

        builder.HasDiscriminatorInJsonId();
    }
}
