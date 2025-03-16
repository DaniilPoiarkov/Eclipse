using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eclipse.DataAccess.Migrations;

internal sealed class VersionInfoConfiguration : IEntityTypeConfiguration<VersionInfo>
{
    public void Configure(EntityTypeBuilder<VersionInfo> builder)
    {
        builder.HasPartitionKey(x => x.Id);

        builder.Property(x => x.Version)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(x => x.AppliedAt)
            .IsRequired();

        builder.HasDiscriminatorInJsonId();
    }
}
