using Eclipse.Domain.Users;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eclipse.DataAccess.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // TODO: Adjust after migrating db.
        builder.ToContainer("IdentityUsers_dev");
            //.HasNoDiscriminator();

        //builder.Property(u => u.Id)
        //    .ToJsonProperty("id");

        //builder.Property(u => u.UserName)
        //    .ToJsonProperty("Username");
    }
}
