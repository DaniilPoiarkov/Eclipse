using Eclipse.Domain.IdentityUsers;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eclipse.DataAccess.Configurations;

internal sealed class IdentityUserConfiguration : IEntityTypeConfiguration<IdentityUser>
{
    public void Configure(EntityTypeBuilder<IdentityUser> builder)
    {
        builder.ToContainer("IdentityUsers")
            .HasNoDiscriminator()
            .HasKey(u => u.Id);
    }
}
