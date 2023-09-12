namespace Eclipse.Application.Exceptions;

internal class UnableToParseValueException : Exception
{
    public UnableToParseValueException(string source, string destination, Exception? inner = null)
        : base($"Unable to parse {source} to {destination}", inner) { }
}
