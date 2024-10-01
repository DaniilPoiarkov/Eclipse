using Eclipse.DataAccess.CosmosDb;
using Eclipse.Domain.MoodRecords;
using Eclipse.Domain.OutboxMessages;
using Eclipse.Domain.Users;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Eclipse.DataAccess.Model;

internal sealed class ModelBuilderConfigurator : IModelBuilderConfigurator
{
    private readonly IEntityTypeConfiguration<User> _userConfiguration;

    private readonly IEntityTypeConfiguration<OutboxMessage> _outboxMessageConfiguration;

    private readonly IEntityTypeConfiguration<MoodRecord> _moodRecordConfiguration;

    private readonly IOptions<CosmosDbContextOptions> _options;

    public ModelBuilderConfigurator(
        IEntityTypeConfiguration<User> userConfiguration,
        IEntityTypeConfiguration<OutboxMessage> outboundMessageConfiguration,
        IOptions<CosmosDbContextOptions> options,
        IEntityTypeConfiguration<MoodRecord> moodRecordConfiguration)
    {
        _userConfiguration = userConfiguration;
        _outboxMessageConfiguration = outboundMessageConfiguration;
        _options = options;
        _moodRecordConfiguration = moodRecordConfiguration;
    }

    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(_userConfiguration);
        modelBuilder.ApplyConfiguration(_outboxMessageConfiguration);
        modelBuilder.ApplyConfiguration(_moodRecordConfiguration);

        modelBuilder.HasDefaultContainer(_options.Value.Container);
    }
}
