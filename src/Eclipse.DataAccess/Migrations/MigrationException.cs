namespace Eclipse.DataAccess.Migrations;

public sealed class MigrationException : Exception
{
    internal MigrationException(string message, Exception? inner = null)
        : base(message, inner) { }
}
