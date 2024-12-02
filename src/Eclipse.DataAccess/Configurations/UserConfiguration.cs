using Eclipse.DataAccess.CosmosDb;
using Eclipse.Domain.Users;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;

namespace Eclipse.DataAccess.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    private readonly IOptions<CosmosDbContextOptions> _options;

    public UserConfiguration(IOptions<CosmosDbContextOptions> options)
    {
        _options = options;
    }

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToContainer(_options.Value.Container)
            .HasPartitionKey(u => u.Id);

        builder.HasDiscriminator<string>("Discriminator")
            .HasValue(nameof(User));

        builder.HasDiscriminatorInJsonId();
    }
}
