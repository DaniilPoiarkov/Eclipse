using Eclipse.Domain.Users;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eclipse.DataAccess.Users;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasPartitionKey(u => u.Id);

        builder.HasDiscriminator<string>("Discriminator")
            .HasValue(nameof(User));

        builder.HasDiscriminatorInJsonId();
    }
}
