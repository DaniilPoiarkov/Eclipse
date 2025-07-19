namespace Eclipse.DataAccess.Migrations;

internal interface IMigrationRunner
{
    Task Migrate(CancellationToken cancellationToken = default);
}
