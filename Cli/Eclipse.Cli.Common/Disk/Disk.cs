
namespace Eclipse.Cli.Common.Disk;

internal sealed class Disk : IDisk
{
    private static readonly string _basePath = Environment.GetFolderPath(
        Environment.SpecialFolder.ApplicationData
    );

    public Task ReadAsync(string path, string fileName, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, path);

        if (!Directory.Exists(fullPath))
        {
            return Task.FromResult(string.Empty);
        }

        return File.ReadAllTextAsync(
            Path.Combine(fullPath, fileName),
            cancellationToken
        );
    }

    public Task WriteAsync(string path, string fileName, string value, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, path);

        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }

        return File.WriteAllTextAsync(
            Path.Combine(fullPath, fileName),
            value,
            cancellationToken
        );
    }
}
