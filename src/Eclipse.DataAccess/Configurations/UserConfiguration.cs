using Eclipse.DataAccess.Constants;
using Eclipse.Domain.Users;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eclipse.DataAccess.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToContainer(ContainerNames.Aggregates)
            .HasPartitionKey(u => u.Id);
    }
}
