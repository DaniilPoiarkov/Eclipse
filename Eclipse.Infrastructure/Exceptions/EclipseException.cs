namespace Eclipse.Infrastructure.Exceptions;

public class EclipseException : Exception
{
    public string? Code { get; }

    public EclipseException(string? message = null, string? code = null) : base(message)
    {
        Code = code;
    }

    public EclipseException WithData(string key, object value)
    {
        Data.Add(key, value);
        return this;
    }
}
