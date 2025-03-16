namespace Eclipse.DataAccess.Migrations;

internal interface IMigration
{
    Task Migrate(CancellationToken cancellationToken = default);
}
