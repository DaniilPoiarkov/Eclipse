namespace Eclipse.Cli.Common.Disk;

public interface IDisk
{
    Task ReadAsync(string path, string fileName, CancellationToken cancellationToken = default);

    Task WriteAsync(string path, string fileName, string value, CancellationToken cancellationToken = default);
}
