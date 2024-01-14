namespace Eclipse.Application.Google.Sheets;

internal class UnableToParseValueException : Exception
{
    public UnableToParseValueException(string source, string destination, Exception? inner = null)
        : base($"Unable to parse {source} to {destination}", inner) { }
}
