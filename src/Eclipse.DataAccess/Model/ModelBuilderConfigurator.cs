using Eclipse.DataAccess.CosmosDb;
using Eclipse.Domain.OutboxMessages;
using Eclipse.Domain.Users;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Eclipse.DataAccess.Model;

internal sealed class ModelBuilderConfigurator : IModelBuilderConfigurator
{
    private readonly IEntityTypeConfiguration<User> _userConfiguration;

    private readonly IEntityTypeConfiguration<OutboxMessage> _outboundMessageConfiguration;

    private readonly IOptions<CosmosDbContextOptions> _options;

    public ModelBuilderConfigurator(
        IEntityTypeConfiguration<User> userConfiguration,
        IEntityTypeConfiguration<OutboxMessage> outboundMessageConfiguration,
        IOptions<CosmosDbContextOptions> options)
    {
        _userConfiguration = userConfiguration;
        _outboundMessageConfiguration = outboundMessageConfiguration;
        _options = options;
    }

    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(_userConfiguration);
        modelBuilder.ApplyConfiguration(_outboundMessageConfiguration);

        modelBuilder.HasDefaultContainer(_options.Value.Container);
    }
}
