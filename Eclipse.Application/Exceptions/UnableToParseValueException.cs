namespace Eclipse.Application.Exceptions;

internal class UnableToParseValueException : ApplicationException
{
    public UnableToParseValueException(string source, string destination, Exception? inner = null)
        : base($"Unable to parse {source} to {destination}", inner) { }
}
