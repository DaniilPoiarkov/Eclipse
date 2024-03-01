namespace Eclipse.Cli.Common.IO;

public interface IInputReader
{
    Task<string> ReadAsync(CancellationToken cancellationToken = default);
}

public interface IOutputWriter
{
    Task WriteAsync(string value, CancellationToken cancellationToken = default);
}

public interface IWindow : IInputReader, IOutputWriter
{

}
