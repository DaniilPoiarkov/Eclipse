using Eclipse.Localization.Exceptions;

namespace Eclipse.Application.Exceptions;

public class EclipseValidationException : LocalizedException
{
    private readonly IList<string> _errors;

    public IReadOnlyList<string> Errors => _errors.AsReadOnly();

    public EclipseValidationException(IList<string> errors) : base("Eclipse:ValidationFailed", errors.ToArray())
    {
        _errors = errors;
    }
}
