using Eclipse.Domain.IdentityUsers;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eclipse.DataAccess.Configurations;

internal sealed class IdentityUserConfiguration : IEntityTypeConfiguration<IdentityUser>
{
    public void Configure(EntityTypeBuilder<IdentityUser> builder)
    {
        // TODO: Adjust after migrating db.
        builder.ToContainer("IdentityUsers")
            .HasNoDiscriminator();

        builder.Property(u => u.Id)
            .ToJsonProperty("id");

        builder.Property(u => u.UserName)
            .ToJsonProperty("Username");
    }
}
