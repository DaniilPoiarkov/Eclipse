namespace Eclipse.Application.Exceptions;

public class EclipseValidationException : Exception
{
    private readonly IList<string> _errors;

    public IReadOnlyList<string> Errors => _errors.AsReadOnly();

    public EclipseValidationException(IList<string> errors)
    {
        _errors = errors;
    }
}
