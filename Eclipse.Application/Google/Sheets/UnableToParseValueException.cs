namespace Eclipse.Application.Google.Sheets;

[Serializable]
internal sealed class UnableToParseValueException : Exception
{
    public UnableToParseValueException(string source, string destination, Exception? inner = null)
        : base($"Unable to parse {source} to {destination}", inner) { }
}
