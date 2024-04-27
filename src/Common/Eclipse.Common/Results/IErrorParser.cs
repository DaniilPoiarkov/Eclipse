namespace Eclipse.Common.Results;

public interface IErrorParser
{
    int StatusCode { get; }

    string Title { get; }

    string Type { get; }
}
