using System.Text;

namespace Eclipse.Cli.Common.IO.Console;

internal sealed class ConsoleWindow : IWindow
{
    private readonly IInputReader _input;

    private readonly IOutputWriter _output;

    public ConsoleWindow(IInputReader input, IOutputWriter output)
    {
        _input = input;
        _output = output;
    }

    public Task<string> ReadAsync(CancellationToken cancellationToken = default)
    {
        return _input.ReadAsync(cancellationToken);
    }

    public Task WriteAsync(string value, CancellationToken cancellationToken = default)
    {
        return _output.WriteAsync(value, cancellationToken);
    }
}

internal sealed class ConsoleReader : IInputReader
{
    public async Task<string> ReadAsync(CancellationToken cancellationToken = default)
    {
        return await System.Console.In.ReadLineAsync(cancellationToken) ?? string.Empty;
    }
}

internal sealed class ConsoleWriter : IOutputWriter
{
    public Task WriteAsync(string value, CancellationToken cancellationToken = default)
    {
        return System.Console.Out.WriteLineAsync(
            new StringBuilder(value),
            cancellationToken
        );
    }
}
