namespace Eclipse.Domain.Exceptions.Base;

internal class BusinessException : Exception
{
    public BusinessException(string? message = null, Exception? inner = null)
        : base(message, inner) { }
}
